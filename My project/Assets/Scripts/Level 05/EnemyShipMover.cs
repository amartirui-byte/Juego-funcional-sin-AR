using UnityEngine;

public class EnemyShipMover : MonoBehaviour
{
    [Header("Velocidades")]
    public float horizontalSpeed = 4f;
    public float verticalSpeed = 2f;

    float verticalDir;

    void Start()
    {
        // Algunas suben, otras bajan
        verticalDir = Random.value > 0.5f ? 1f : -1f;
    }

    void Update()
    {
        // Movimiento diagonal (izquierda + arriba/abajo)
        Vector3 dir = new Vector3(-horizontalSpeed, verticalDir * verticalSpeed, 0f);
        transform.position += dir * Time.deltaTime;

        // Si se va fuera, se destruye
        if (transform.position.x < -20f || Mathf.Abs(transform.position.y) > 15f)
            Destroy(gameObject);
    }
}
