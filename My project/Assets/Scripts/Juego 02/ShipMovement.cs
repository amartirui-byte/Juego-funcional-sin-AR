using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipMovement : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 10f;   // qué rápido acelera
    public float maxSpeed = 6f;        // velocidad máxima
    public float deceleration = 8f;    // cómo de rápido frena

    [Header("Touch / Pointer control")]
    [Tooltip("Zona muerta en unidades de mundo (si el dedo está muy cerca de la nave, no se mueve).")]
    public float deadZoneWorld = 0.15f;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 inputDir = Vector2.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        inputDir = ReadMoveDirection();
    }

    private Vector2 ReadMoveDirection()
    {
        // 1) Touch (Android/iOS) o Mouse (Editor/PC): mover hacia el puntero mientras esté pulsado
        if (cam != null)
        {
            Vector2 screenPos;
            bool pressed = false;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                pressed = true;
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                pressed = true;
                screenPos = Mouse.current.position.ReadValue();
            }
            else
            {
                screenPos = default;
            }

            if (pressed)
            {
                float zDist = -cam.transform.position.z;
                Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDist));
                Vector2 delta = (Vector2)world - rb.position;

                if (delta.magnitude <= deadZoneWorld)
                    return Vector2.zero;

                return delta.normalized;
            }
        }

        // 2) Fallback teclado (para probar en PC) usando NEW Input System
        Vector2 k = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) k.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) k.x += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) k.y -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) k.y += 1f;
        }

        return k.normalized;
    }

    void FixedUpdate()
    {
        Vector2 vel = rb.velocity;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            vel += inputDir * acceleration * Time.fixedDeltaTime;
            vel = Vector2.ClampMagnitude(vel, maxSpeed);
        }
        else
        {
            vel = Vector2.MoveTowards(vel, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.velocity = vel;
    }
}