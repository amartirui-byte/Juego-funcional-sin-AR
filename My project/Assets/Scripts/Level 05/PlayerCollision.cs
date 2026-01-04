using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.CompareTag("Obstacle"))
        {
            Debug.Log("Has chocado con un obstáculo");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Star"))
        {
            Debug.Log("Has cogido una estrella");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddStar();
            }

            Destroy(other.gameObject);
        }
    }
}
