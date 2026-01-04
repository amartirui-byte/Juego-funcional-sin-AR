using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab;

    [Header("Spawn")]
    public float spawnIntervalMin = 2f;
    public float spawnIntervalMax = 4f;

    // Rango general de altura
    public Vector2 yRange = new Vector2(-4f, 4f);

    // Rango fácil (más cerca del centro)
    public Vector2 easyYRange = new Vector2(-1.5f, 1.5f);

    public int totalStars = 10;
    public int easyStars = 5;

    int spawnedStars = 0;
    float nextSpawnIn;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (spawnedStars >= totalStars) return;

        nextSpawnIn -= Time.deltaTime;
        if (nextSpawnIn <= 0f)
        {
            SpawnStar();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        nextSpawnIn = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    void SpawnStar()
    {
        if (spawnedStars >= totalStars) return;

        Vector2 rangeToUse;

        // Las primeras "easyStars" salen en el rango fácil
        if (spawnedStars < easyStars)
            rangeToUse = easyYRange;
        else
            rangeToUse = yRange;

        float y = Random.Range(rangeToUse.x, rangeToUse.y);
        Vector3 pos = new Vector3(transform.position.x, y, 0f);

        Instantiate(starPrefab, pos, Quaternion.identity);
        spawnedStars++;
    }
}
