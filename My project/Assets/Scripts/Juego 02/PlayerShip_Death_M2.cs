using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerShip_Death_M2 : MonoBehaviour
{
    [Header("Muerte / FX")]
    public GameObject explosionPrefab;

    [Tooltip("Tag que deben tener los asteroides.")]
    public string asteroidTag = "Asteroid";

    [Tooltip("Tiempo (real) para dejar ver la explosión antes del GameOver.")]
    public float deathDelayRealtime = 0.3f;

    [Tooltip("Si quieres, destruye el FX pasado este tiempo (en segundos). 0 = no destruir.")]
    public float destroyExplosionAfter = 2f;

    private bool dead = false;

    private Rigidbody2D rb;
    private Collider2D[] allColliders;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        allColliders = GetComponents<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Si los asteroides NO son trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    // Si los asteroides SÍ son trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject other)
    {
        if (dead) return;
        if (other == null) return;

        // El tag del asteroide debe coincidir con el prefab/escena
        if (!other.CompareTag(asteroidTag)) return;

        dead = true;

        // Evitar más colisiones mientras muere
        if (allColliders != null)
        {
            for (int i = 0; i < allColliders.Length; i++)
            {
                if (allColliders[i] != null) allColliders[i].enabled = false;
            }
        }

        StartCoroutine(DieAndGameOver());
    }

    private IEnumerator DieAndGameOver()
    {
        // 1) Explosión
        if (explosionPrefab != null)
        {
            var fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (destroyExplosionAfter > 0f)
                Destroy(fx, destroyExplosionAfter);
        }

        // 2) Ocultar nave
        if (sr != null) sr.enabled = false;

        // 3) Parar movimiento (y evitar que el rigidbody siga “simulando”)
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // 4) Esperar en tiempo real (no depende de Time.timeScale)
        if (deathDelayRealtime > 0f)
            yield return new WaitForSecondsRealtime(deathDelayRealtime);

        // 5) Avisar al GameManager
        if (GameManager_M2.Instance != null)
        {
            GameManager_M2.Instance.GameOver();
        }
        else
        {
            Debug.LogError("[PlayerShip_Death_M2] No existe GameManager_M2.Instance en la escena.");
        }
    }
}

