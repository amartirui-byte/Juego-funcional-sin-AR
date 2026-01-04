using UnityEngine;

public class StarMover : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Si se va muy lejos
        if (transform.position.x < -20f)
            Destroy(gameObject);
    }
}
