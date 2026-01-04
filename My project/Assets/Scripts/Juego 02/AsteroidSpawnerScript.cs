using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject asteroidPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.5f; // segundos entre asteroides
    public float minY = -4f;           // altura mínima
    public float maxY = 4f;            // altura máxima
    public float spawnX = 12f;         // posición X donde aparecen (derecha)

    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnAsteroid();
            timer = spawnInterval;
        }
    }

    void SpawnAsteroid()
    {
        if (asteroidPrefab == null)
        {
            Debug.LogWarning("AsteroidSpawner: asteroidPrefab no asignado");
            return;
        }

        float y = Random.Range(minY, maxY);
        Vector3 pos = new Vector3(spawnX, y, 0f);

        Instantiate(asteroidPrefab, pos, Quaternion.identity);
    }
}
