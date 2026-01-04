using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject rockPrefab;
    public GameObject meteorBigPrefab;
    public GameObject enemyShipPrefab;

    [Header("Spawn")]
    public float spawnIntervalMin = 0.8f;
    public float spawnIntervalMax = 2f;
    public Vector2 yRange = new Vector2(-4f, 4f);

    float nextSpawnIn;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        nextSpawnIn -= Time.deltaTime;
        if (nextSpawnIn <= 0f)
        {
            SpawnObstacle();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        nextSpawnIn = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    void SpawnObstacle()
    {
        float r = Random.value; // número entre 0 y 1

        GameObject prefab;

        if (r < 0.6f)
        {
            // 60% Rocas
            prefab = rockPrefab;
        }
        else if (r < 0.85f)
        {
            // 25% Naves enemigas
            prefab = enemyShipPrefab;
        }
        else
        {
            // 15% Meteoritos grandes
            prefab = meteorBigPrefab;
        }

        float y = Random.Range(yRange.x, yRange.y);
        Vector3 pos = new Vector3(transform.position.x, y, 0f);

        Instantiate(prefab, pos, Quaternion.identity);
    }
    public void MakeHarder(float factor)
    {
        spawnIntervalMin *= factor;
        spawnIntervalMax *= factor;

        spawnIntervalMin = Mathf.Max(0.3f, spawnIntervalMin);
        spawnIntervalMax = Mathf.Max(spawnIntervalMin + 0.1f, spawnIntervalMax);
    }


}
