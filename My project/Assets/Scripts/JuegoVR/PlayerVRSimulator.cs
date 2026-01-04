using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerVRSimulator : MonoBehaviour
{
    [Header("Referencias (PC Sim)")]
    [Tooltip("Cámara que actúa como 'cabeza' (Main Camera).")]
    public Transform cameraPivot;

    [Tooltip("Objeto que rota en Y (yaw). Si lo dejas vacío, usa este GameObject.")]
    public Transform yawRoot;

    [Header("Ajustes (PC Sim)")]
    public float sensibilidad = 0.15f;
    public bool lockCursor = true;

    [Tooltip("Si está activado, desactiva cualquier TrackedPoseDriver en la jerarquía de cameraPivot en PC.")]
    public bool disableTrackedPoseDriversOnPC = true;

    private float yaw;
    private float pitch;

    private PlayerVR player;
    private Rigidbody rb;

    void Awake()
    {
        player = GetComponent<PlayerVR>();
        rb = GetComponent<Rigidbody>();

        if (yawRoot == null)
            yawRoot = transform;
    }

    void OnEnable()
    {
        // En móvil: este script NO debe actuar
        if (Application.isMobilePlatform)
        {
            enabled = false;
            return;
        }

        if (cameraPivot == null)
        {
            Debug.LogError("[PlayerVRSimulator] cameraPivot no asignado. Arrastra la Main Camera aquí.");
            enabled = false;
            return;
        }

        // Inicializa acumuladores desde rotación actual
        yaw = yawRoot.eulerAngles.y;

        pitch = cameraPivot.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (disableTrackedPoseDriversOnPC)
            DisableTrackedPoseDrivers(cameraPivot);
    }

    void Update()
    {
        if (Application.isMobilePlatform) return;
        if (!Application.isFocused) return;

        if (disableTrackedPoseDriversOnPC)
            DisableTrackedPoseDrivers(cameraPivot);

        Vector2 d = ReadMouseDelta();

        // Si no llega movimiento, no rotamos (pero el clic puede funcionar)
        if (d != Vector2.zero)
        {
            yaw += d.x * sensibilidad;
            pitch -= d.y * sensibilidad;
            pitch = Mathf.Clamp(pitch, -85f, 85f);

            // Pitch en la cámara (local)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // Disparo con clic
        if (IsFirePressed())
            player?.IntentarDisparar();

        // Escape libera el ratón
        if (IsEscapePressed())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        if (Application.isMobilePlatform) return;

        Quaternion targetYaw = Quaternion.Euler(0f, yaw, 0f);

        // Yaw: si el yawRoot tiene Rigidbody, usa física; si no, asigna rotación directamente
        if (rb != null && rb.transform == yawRoot && !rb.isKinematic)
            rb.MoveRotation(targetYaw);
        else
            yawRoot.rotation = targetYaw;
    }

    // ---------- Input helpers ----------

    private Vector2 ReadMouseDelta()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            // Delta real funciona con cursor bloqueado
            return Mouse.current.delta.ReadValue();
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        // Fallback legacy
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");
        return new Vector2(mx, my) * 10f; // escala para parecerse al delta en píxeles
#else
        return Vector2.zero;
#endif
    }

    private bool IsFirePressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0))
            return true;
#endif
        return false;
    }

    private bool IsEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame);
#else
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Escape);
#else
        return false;
#endif
#endif
    }

    private void DisableTrackedPoseDrivers(Transform root)
    {
        if (root == null) return;

        // Desactiva cualquier componente cuyo tipo contenga "TrackedPoseDriver"
        var behaviours = root.GetComponentsInChildren<Behaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            var b = behaviours[i];
            if (b == null) continue;

            if (b.enabled && b.GetType().Name.Contains("TrackedPoseDriver"))
                b.enabled = false;
        }
    }
}
