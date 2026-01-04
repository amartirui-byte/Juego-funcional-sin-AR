using UnityEngine;

public class AsteroidFall_M2 : MonoBehaviour
{
    public float speed = 1.5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.velocity = new Vector2(0f, -speed);

        // Cuando el asteroide sale por abajo, sumamos 1 punto y lo destruimos
        if (transform.position.y < -6f)
        {
            if (GameManager_M2.Instance != null)
            {
                GameManager_M2.Instance.AddScore(1);
            }

            Destroy(gameObject);
        }
    }
}
