using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerShipController : MonoBehaviour
{
    [Header("Movimiento")]
    public float acceleration = 20f;
    public float maxSpeed = 6f;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;

    [Header("Control táctil")]
    public float deadZoneWorld = 0.25f;
    public float doubleTapWindow = 0.28f;

    [Header("Soporte Editor/PC (opcional)")]
    public bool enableMouseInEditor = true;
    public bool enableKeyboardInEditor = true;

    private Rigidbody2D rb;
    private Camera cam;

    private Vector2 inputDir;
    private Vector2 lastMoveDir = Vector2.right;

    private bool isDashing = false;
    private float dashEndTime = 0f;
    private float dashCooldownEndTime = 0f;

    private float lastTapTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        if (cam == null) cam = Camera.main;

        inputDir = Vector2.zero;

        // ====== MOVIL (Touchscreen) ======
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed)
            {
                Vector2 screenPos = touch.position.ReadValue();
                Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

                Vector2 dir = worldPos - (Vector2)transform.position;
                if (dir.magnitude > deadZoneWorld)
                {
                    inputDir = dir.normalized;
                    lastMoveDir = inputDir;
                }
            }

            if (touch.press.wasPressedThisFrame)
                TryDoubleTapDash();
        }
        // ====== EDITOR/PC (Mouse) ======
        else if (enableMouseInEditor && Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

                Vector2 dir = worldPos - (Vector2)transform.position;
                if (dir.magnitude > deadZoneWorld)
                {
                    inputDir = dir.normalized;
                    lastMoveDir = inputDir;
                }
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
                TryDoubleTapDash();
        }

        // ====== TECLADO (solo para probar en PC, New Input System) ======
        if (enableKeyboardInEditor && Keyboard.current != null)
        {
            Vector2 kb = Vector2.zero;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) kb.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) kb.x += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) kb.y -= 1;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) kb.y += 1;

            if (kb != Vector2.zero)
            {
                inputDir = kb.normalized;
                lastMoveDir = inputDir;
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                TryDash();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = lastMoveDir * dashSpeed;
            if (Time.time >= dashEndTime) isDashing = false;
            return;
        }

        if (inputDir != Vector2.zero)
        {
            rb.AddForce(inputDir * acceleration);

            if (rb.velocity.magnitude > maxSpeed)
                rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.1f);
        }
    }

    private void TryDoubleTapDash()
    {
        float t = Time.unscaledTime;

        if (t - lastTapTime <= doubleTapWindow)
        {
            TryDash();
            lastTapTime = -999f;
        }
        else
        {
            lastTapTime = t;
        }
    }

    private void TryDash()
    {
        if (Time.time < dashCooldownEndTime) return;

        isDashing = true;
        dashEndTime = Time.time + dashDuration;
        dashCooldownEndTime = Time.time + dashCooldown;
    }

    public void DashButton()
    {
        TryDash();
    }
}
