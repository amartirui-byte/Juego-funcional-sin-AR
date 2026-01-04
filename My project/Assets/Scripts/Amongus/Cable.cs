using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Cable : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer finalCable;
    public GameObject luz;

    private Vector2 posicionOriginal;
    private Vector2 tamanoOriginal;
    private TareaCables tareaCables;

    private Camera cam;
    private Collider2D col;
    private Vector2 puntoOrigen;

    private bool arrastrando = false;

    // Para controlar un toque concreto en movil
    private int activeTouchId = -1;

    void Start()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();

        if (cam == null)
        {
            Debug.LogError("[Cable] No hay Camera.main (la camara debe tener tag MainCamera).");
            enabled = false;
            return;
        }

        if (finalCable == null)
        {
            Debug.LogError("[Cable] Asigna 'finalCable' (SpriteRenderer) en el inspector.");
            enabled = false;
            return;
        }

        posicionOriginal = transform.position;
        tamanoOriginal = finalCable.size;

        tareaCables = transform.root.GetComponent<TareaCables>();
        puntoOrigen = transform.parent.position;
    }

    void Update()
    {
        ProcesarInputNew();

        if (arrastrando)
            ProcesarArrastre();
    }

    private void ProcesarInputNew()
    {
        // ===== MOVIL (Touch) =====
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                Vector2 screenPos = touch.position.ReadValue();
                Vector2 worldPos = ScreenToWorld2D(screenPos);

                // Solo empezamos si tocamos ESTE cable
                if (Physics2D.OverlapPoint(worldPos) == col)
                {
                    arrastrando = true;
                    activeTouchId = touch.touchId.ReadValue();
                }
            }

            if (arrastrando && touch.press.wasReleasedThisFrame)
            {
                // Si se suelta el primary touch, reseteamos
                Reiniciar();
                arrastrando = false;
                activeTouchId = -1;
            }

            return; // si hay Touch, no miramos raton
        }

        // ===== PC (Mouse) =====
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector2 worldPos = ScreenToWorld2D(screenPos);

            if (Physics2D.OverlapPoint(worldPos) == col)
                arrastrando = true;
        }

        if (arrastrando && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Reiniciar();
            arrastrando = false;
        }
    }

    private void ProcesarArrastre()
    {
        Vector2 screenPos;

        if (Touchscreen.current != null)
        {
            // En movil arrastramos con el primary touch
            var touch = Touchscreen.current.primaryTouch;

            if (!touch.press.isPressed)
            {
                Reiniciar();
                arrastrando = false;
                activeTouchId = -1;
                return;
            }

            screenPos = touch.position.ReadValue();
        }
        else
        {
            // PC
            if (Mouse.current == null || !Mouse.current.leftButton.isPressed)
            {
                Reiniciar();
                arrastrando = false;
                return;
            }

            screenPos = Mouse.current.position.ReadValue();
        }

        Vector2 worldPos = ScreenToWorld2D(screenPos);

        ActualizarPosicion(worldPos);
        ActualizarRotacion();
        ActualizarTamano();
        ComprobarConexion();
    }

    private Vector2 ScreenToWorld2D(Vector2 screenPos)
    {
        float z = -cam.transform.position.z; // plano 2D
        Vector3 wp = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
        return new Vector2(wp.x, wp.y);
    }

    private void ActualizarPosicion(Vector2 nuevaPos)
    {
        transform.position = nuevaPos;
    }

    private void ActualizarRotacion()
    {
        Vector2 dir = (Vector2)transform.position - puntoOrigen;

        if (dir.sqrMagnitude > 0.0001f)
        {
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, ang);
        }
    }

    private void ActualizarTamano()
    {
        float dist = Vector2.Distance(transform.position, puntoOrigen);
        finalCable.size = new Vector2(dist, tamanoOriginal.y);
    }

    private void Reiniciar()
    {
        transform.position = posicionOriginal;
        transform.rotation = Quaternion.identity;
        finalCable.size = tamanoOriginal;
    }

    private void ComprobarConexion()
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, 0.01f);

        foreach (Collider2D c in colisiones)
        {
            if (c == null) continue;
            if (c.gameObject == gameObject) continue;

            Cable otro = c.GetComponent<Cable>();
            if (otro == null) continue;

            // Ajusta al punto de conexion
            transform.position = c.transform.position;

            // Solo conecta si el color coincide
            if (finalCable.color == otro.finalCable.color)
            {
                Conectar();
                otro.Conectar();

                if (tareaCables != null)
                {
                    tareaCables.conexionesActuales++;
                    tareaCables.ComprobarVictoria();
                }

                return;
            }
        }
    }

    public void Conectar()
    {
        if (luz != null) luz.SetActive(true);
        arrastrando = false;
        activeTouchId = -1;
        Destroy(this);
    }
}
