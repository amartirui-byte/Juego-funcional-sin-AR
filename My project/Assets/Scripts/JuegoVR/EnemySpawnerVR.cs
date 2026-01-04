using UnityEngine;

public class EnemySpawnerVR : MonoBehaviour
{
    [Header("Enemigos y spawn points")]
    public GameObject enemigoPrefab;
    public Transform[] spawnPoints;

    [Header("Spawn settings")]
    public float intervaloSpawnInicial = 1f;
    public float intervaloSpawnMinimo = 1f;
    public float decrementoIntervalo = 1f;
    public int maxEnemigos = 10;

    private int enemiesSpawned = 0;
    private float timer = 0f;

    void Update()
    {
        if (enemiesSpawned >= maxEnemigos)
            return;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No hay spawn points asignados!");
            return;
        }

        timer += Time.deltaTime;
        if (timer >= intervaloSpawnInicial)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (enemiesSpawned >= maxEnemigos)
            return;

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        if (spawnPoint == null)
        {
            Debug.LogWarning("EnemySpawner: Spawn point nulo en índice " + index);
            return;
        }

        Instantiate(enemigoPrefab, spawnPoint.position, spawnPoint.rotation);
        enemiesSpawned++;
    }
}
