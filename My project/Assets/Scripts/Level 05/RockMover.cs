using UnityEngine;

public class RockMover : MonoBehaviour
{
    public float horizontalSpeed = 3f;    // se mueve a la izquierda
    public float verticalAmplitude = 1f;  // cuánto sube/baja
    public float verticalFrequency = 2f;  // velocidad del movimiento vertical

    float startY;
    float timeOffset;

    void Start()
    {
        startY = transform.position.y;
        timeOffset = Random.value * 10f; // que no todas vayan sincronizadas
    }

    void Update()
    {
        // Movimiento horizontal
        transform.position += Vector3.left * horizontalSpeed * Time.deltaTime;

        // Movimiento vertical (arriba–abajo)
        float y = startY + Mathf.Sin((Time.time + timeOffset) * verticalFrequency) * verticalAmplitude;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        // Destruir si sale mucho de la pantalla
        if (transform.position.x < -20f)
            Destroy(gameObject);
    }
}

