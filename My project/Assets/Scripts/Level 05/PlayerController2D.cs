using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad máxima en unidades/segundo.")]
    public float maxSpeed = 10f;

    [Tooltip("Tiempo de suavizado. Más bajo = más reactivo. Más alto = más suave.")]
    public float smoothTime = 0.06f;

    [Tooltip("Zona muerta en mundo para evitar micro ajustes.")]
    public float deadZoneWorld = 0.12f;

    private Rigidbody2D rb;
    private Camera cam;

    private Vector2 targetWorld;
    private bool hasTarget;

    private Vector2 smoothVel; // usado por SmoothDamp

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        // Recomendado para movimiento suave visual
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        if (TryGetPointer(out Vector2 screenPos, out bool pressed, out bool pressedThisFrame, out bool releasedThisFrame))
        {
            if (pressed || pressedThisFrame)
            {
                targetWorld = ScreenToWorld2D(screenPos);
                hasTarget = true;
            }

            if (releasedThisFrame)
            {
                hasTarget = false;
                smoothVel = Vector2.zero; // evita “rebotes” al soltar
            }
        }
    }

    void FixedUpdate()
    {
        if (!hasTarget)
            return;

        Vector2 current = rb.position;
        Vector2 toTarget = targetWorld - current;

        if (toTarget.magnitude <= deadZoneWorld)
            return;

        // Suaviza el movimiento hacia el objetivo y limita velocidad
        Vector2 next = Vector2.SmoothDamp(
            current,
            targetWorld,
            ref smoothVel,
            smoothTime,
            maxSpeed,
            Time.fixedDeltaTime
        );

        rb.MovePosition(next);
    }

    private bool TryGetPointer(out Vector2 screenPos, out bool pressed, out bool pressedThisFrame, out bool releasedThisFrame)
    {
        screenPos = Vector2.zero;
        pressed = false;
        pressedThisFrame = false;
        releasedThisFrame = false;

        // Móvil
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            screenPos = t.position.ReadValue();
            pressed = t.press.isPressed;
            pressedThisFrame = t.press.wasPressedThisFrame;
            releasedThisFrame = t.press.wasReleasedThisFrame;
            return true;
        }

        // PC
        if (Mouse.current != null)
        {
            screenPos = Mouse.current.position.ReadValue();
            pressed = Mouse.current.leftButton.isPressed;
            pressedThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
            releasedThisFrame = Mouse.current.leftButton.wasReleasedThisFrame;
            return true;
        }

        return false;
    }

    private Vector2 ScreenToWorld2D(Vector2 screenPos)
    {
        // En ortográfica el X/Y salen bien; forzamos Z=0 por seguridad en 2D
        Vector3 wp = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z));
        return new Vector2(wp.x, wp.y);
    }
}
