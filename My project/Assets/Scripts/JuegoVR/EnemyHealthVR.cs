using UnityEngine;

public class EnemyHealthVR : MonoBehaviour
{
    public int vida = 1;

    public void RecibirDaño(int cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            GameManagerVR.Instance.EnemigoDestruido();
        }
    }
}