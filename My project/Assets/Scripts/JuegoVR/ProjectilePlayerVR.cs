using UnityEngine;

public class ProjectilePlayerVR : MonoBehaviour
{
    public float velocidad = 30f;
    public float vida = 3f;

    private void Start()
    {
        Destroy(gameObject, vida);
    }

    private void Update()
    {
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyVR enemy = other.GetComponent<EnemyVR>();
        if (enemy != null)
            enemy.Morir();

        Destroy(gameObject);
    }
}