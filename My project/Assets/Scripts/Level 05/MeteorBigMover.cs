using UnityEngine;

public class MeteorBigMover : MonoBehaviour
{
    public float speed = 3f; // Igual a la velocidad del scroll

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < -20f)
            Destroy(gameObject);
    }
}
