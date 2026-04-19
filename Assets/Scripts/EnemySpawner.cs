using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private float bossSpawnDelay = 2f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnY = 2.5f;
    [SerializeField] private float bossSpawnY = 1.5f;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;

    private ObjectPool enemyPool;
    private int currentEnemyCount = 0;
    private int waveNumber = 1;
    private bool isSpawning = false;
    private const int ENEMIES_PER_WAVE = 3;
    public int BOSS_WAVE_INTERVAL = 10;

    private void Awake()
    {
        GameObject poolObj = new GameObject("EnemyPool");
        poolObj.transform.SetParent(transform);
        enemyPool = poolObj.AddComponent<ObjectPool>();
        enemyPool.Initialize(enemyPrefab, initialPoolSize);
    }

    private void Start()
    {
        isSpawning = true;
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        while (isSpawning)
        {
            if (currentEnemyCount == 0)
            {
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.SetWave(waveNumber);
                }

                if (waveNumber % BOSS_WAVE_INTERVAL == 0)
                {
                    yield return StartCoroutine(SpawnBoss());
                }
                else
                {
                    yield return StartCoroutine(SpawnEnemyWave(ENEMIES_PER_WAVE));
                }
                waveNumber++;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator SpawnEnemyWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float xPos = Random.Range(minX, maxX);
            Vector2 spawnPos = new Vector2(xPos, spawnY);

            GameObject enemy = enemyPool.GetObject();
            if (enemy != null)
            {
                enemy.transform.position = spawnPos;
                enemy.transform.rotation = Quaternion.identity;
                enemy.SetActive(true);
                currentEnemyCount++;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(bossSpawnDelay);

        if (bossPrefab != null)
        {
            float xPos = Random.Range(minX, maxX);
            Vector2 spawnPos = new Vector2(xPos, bossSpawnY);

            GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
            currentEnemyCount++;
            Debug.Log("Boss spawned! Wave: " + waveNumber);
        }
    }

    public void OnEnemyDestroyed()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
        Debug.Log("Enemy destroyed! Remaining: " + currentEnemyCount);
    }

    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public int GetCurrentEnemyCount()
    {
        return currentEnemyCount;
    }
}
