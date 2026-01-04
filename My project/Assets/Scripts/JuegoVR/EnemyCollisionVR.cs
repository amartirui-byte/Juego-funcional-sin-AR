using UnityEngine;

public class EnemyCollisionVR : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerVR>(out PlayerVR player))
        {
            // Daño al jugador
            GameManagerVR.Instance.GameOver();

            // Destruir enemigo
            Destroy(gameObject);
        }
    }
}
