using UnityEngine;

public class Asteroid_MiniJuego2 : MonoBehaviour
{
    public float speed = 3f;                        // Velocidad
    public Vector2 direction = new Vector2(0, -1);  // Dirección por defecto: hacia abajo

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        direction = direction.normalized;
    }

    void Update()
    {
        rb.velocity = direction * speed;

        // Rotación opcional
        transform.Rotate(0f, 0f, 120f * Time.deltaTime);

        // Si se va por abajo de la pantalla, se destruye
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca con la nave (Player)
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Asteroide golpeó la nave. GAME OVER");

            // --- MECÁNICA ANTIGUA (comentada) ---
            // Reiniciar la escena actual
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // -------------------------------------

            if (GameManager_M2.Instance != null)
            {
                GameManager_M2.Instance.GameOver();
            }
            else
            {
                Debug.LogError("[Asteroid_MiniJuego2] No existe GameManager_M2.Instance en la escena.");
            }

            // Evita múltiples colisiones en el mismo frame
            // (por si el asteroide sigue chocando mientras carga la escena)
            GetComponent<Collider2D>().enabled = false;
        }
    }
}

