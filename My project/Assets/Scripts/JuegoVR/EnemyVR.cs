using UnityEngine;

public class EnemyVR : MonoBehaviour
{
    public int dañoAlJugador = 1;

    private bool muerto = false;

    public void Morir()
    {
        if (muerto) return;
        muerto = true;

        if (GameManagerVR.Instance != null)
            GameManagerVR.Instance.EnemigoDestruido();

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerVR player = collision.gameObject.GetComponent<PlayerVR>();
            if (player != null)
                player.RecibirDaño(dañoAlJugador);

            Morir();
        }
    }
}