using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float velocidad = 3f;
    public Transform jugador;

    void Start()
    {
        if (jugador == null)
            jugador = GameManagerVR.Instance.jugador;
    }

    void Update()
    {
        if (jugador == null) return;

        Vector3 haciaJugador = jugador.position - transform.position;

        Debug.DrawLine(transform.position, jugador.position, Color.red);

        Vector3 direccion = haciaJugador.normalized;

        transform.position += direccion * velocidad * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direccion);
    }
}