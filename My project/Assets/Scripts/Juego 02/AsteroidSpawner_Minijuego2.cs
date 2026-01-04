using UnityEngine;

public class AsteroidSpawner_MiniJuego2 : MonoBehaviour
{
    [Header("Prefab del asteroide")]
    public GameObject asteroidPrefab;

    [Header("Frecuencia de aparición (segundos)")]
    public float spawnInterval = 1.2f;

    [Header("Rango horizontal (X)")]
    public float minX = -4f;
    public float maxX = 4f;

    [Header("Velocidad de los asteroides")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnAsteroid();
        }
    }

    void SpawnAsteroid()
    {
        // POSICIÓN ALEATORIA EN X (horizontal)
        float randomX = Random.Range(minX, maxX);

        // El spawner está ARRIBA, así que usamos su Y y el X aleatorio
        Vector3 spawnPos = new Vector3(randomX, transform.position.y, 0f);

        // Crear el asteroide
        GameObject newAsteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        // Dar velocidad y dirección hacia ABAJO
        Asteroid_MiniJuego2 asteroid = newAsteroid.GetComponent<Asteroid_MiniJuego2>();
        if (asteroid != null)
        {
            asteroid.speed = Random.Range(minSpeed, maxSpeed);
            asteroid.direction = new Vector2(0f, -1f); // abajo
        }
    }
}
