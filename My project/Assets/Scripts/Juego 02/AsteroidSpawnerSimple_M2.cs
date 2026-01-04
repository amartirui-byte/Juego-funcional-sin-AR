using UnityEngine;

public class AsteroidSpawnerSimple_M2 : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject asteroidPrefab;

    [Header("Zona donde aparecen (X)")]
    public float minX = -4f;
    public float maxX = 4f;

    [Header("Frecuencia de aparición")]
    public float startInterval = 2f;   // al principio: 1 asteroide cada 2 segundos
    public float minInterval = 0.4f;   // máximo de dificultad: 1 cada 0.4 segundos
    public float difficultySpeed = 0.05f; // qué rápido se reduce el intervalo

    private float currentInterval;
    private float timer = 0f;

    void Start()
    {
        currentInterval = startInterval;
    }

    void Update()
    {
        // 1) Contamos el tiempo desde el último asteroide
        timer += Time.deltaTime;

        // 2) Si ha pasado el intervalo actual, generamos uno
        if (timer >= currentInterval)
        {
            timer = 0f;
            SpawnAsteroid();
        }

        // 3) Cada frame vamos reduciendo un poco el intervalo (más dificultad)
        //    Nunca bajará de minInterval
        currentInterval = Mathf.Max(minInterval, currentInterval - difficultySpeed * Time.deltaTime);
    }

    void SpawnAsteroid()
    {
        if (asteroidPrefab == null)
        {
            Debug.LogError("AsteroidSpawnerSimple_M2: falta asignar asteroidPrefab");
            return;
        }

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y, 0f);

        Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
    }
}
