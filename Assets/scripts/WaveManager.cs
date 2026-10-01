using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [Header("Настройки времени")]
    public float totalGameTime = 510f;        // 8.5 минут
    public float bossTime1 = 120f;            // 2:00
    public float bossTime2 = 240f;            // 4:00
    public float bossTime3 = 360f;            // 6:00
    public float finalBossTime = 510f;        // 8:30

    [Header("Префабы обычных врагов (3 шт)")]
    public GameObject fastEnemyPrefab;
    public GameObject normalEnemyPrefab;
    public GameObject heavyEnemyPrefab;

    [Header("Префабы быстрых вариаций (1 шт)")] // УБРАЛИ ОДНОГО
    public GameObject fastVariant1Prefab;
    // fastVariant2Prefab удалён

    [Header("Префабы искажённых врагов (3 шт)")]
    public GameObject corruptedFastPrefab;
    public GameObject corruptedNormalPrefab;
    public GameObject corruptedHeavyPrefab;    // ЭТОТ ТЕПЕРЬ БУДЕТ РАБОТАТЬ

    [Header("Префабы боссов (4 шт)")]
    public GameObject miniBoss1Prefab;
    public GameObject miniBoss2Prefab;
    public GameObject miniBoss3Prefab;
    public GameObject finalBossPrefab;

    [Header("Настройки спавна - BULLET HEAVEN")]
    public Transform[] spawnPoints;
    public float spawnInterval = 0.2f;
    public int spawnsPerBurst = 5;
    public int baseEnemyCap = 50;
    public int maxEnemyCap = 150;

    [Header("Множители сложности")]
public float healthGrowthPerMinute = 0.5f;    // здоровье растёт медленнее
public float damageGrowthPerMinute = 0.2f;   // урон растёт ВДВОЕ МЕДЛЕННЕЕ
public float spawnRateGrowth = 0.1f;

    private float startTime;
    private bool isBossActive = false;
    private Transform player;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private float currentSpawnInterval;

    private List<GameObject> stage1Enemies;
    private List<GameObject> stage2Enemies;
    private List<GameObject> stage3Enemies;
    private List<GameObject> stage4Enemies;

    private readonly object enemyListLock = new object();

    void Start()
    {
        startTime = Time.time;
        currentSpawnInterval = spawnInterval;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        InitializeEnemyLists();
        StartCoroutine(SpawnRoutine());
        StartCoroutine(BossSpawner());
        if (AudioManager.Instance != null)
        AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
    }

    void InitializeEnemyLists()
    {
        // Стадия 1: только обычные враги
        stage1Enemies = new List<GameObject>();
        if (fastEnemyPrefab != null) stage1Enemies.Add(fastEnemyPrefab);
        if (normalEnemyPrefab != null) stage1Enemies.Add(normalEnemyPrefab);
        if (heavyEnemyPrefab != null) stage1Enemies.Add(heavyEnemyPrefab);

        // Стадия 2: обычные + одна быстрая вариация
        stage2Enemies = new List<GameObject>(stage1Enemies);
        if (fastVariant1Prefab != null) stage2Enemies.Add(fastVariant1Prefab);

        // Стадия 3: всё из стадии 2 + искажённые
        stage3Enemies = new List<GameObject>(stage2Enemies);
        if (corruptedFastPrefab != null) stage3Enemies.Add(corruptedFastPrefab);
        if (corruptedNormalPrefab != null) stage3Enemies.Add(corruptedNormalPrefab);
        if (corruptedHeavyPrefab != null) stage3Enemies.Add(corruptedHeavyPrefab); // ТЕПЕРЬ ДОБАВЛЯЕТСЯ

        // Стадия 4: всё из стадии 3 (можно добавить ещё позже)
        stage4Enemies = new List<GameObject>(stage3Enemies);
    }

    IEnumerator SpawnRoutine()
    {
        while (Time.time - startTime < finalBossTime)
        {
            float elapsed = Time.time - startTime;

            if (isBossActive || player == null || spawnPoints.Length == 0)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Очистка мёртвых врагов
            List<GameObject> deadEnemies = new List<GameObject>();
            lock (enemyListLock)
            {
                foreach (var enemy in activeEnemies)
                {
                    if (enemy == null)
                        deadEnemies.Add(enemy);
                }
                
                foreach (var dead in deadEnemies)
                {
                    activeEnemies.Remove(dead);
                }
            }

            int currentCap = GetCurrentEnemyCap(elapsed);
            int spawnCount = GetSpawnCount(elapsed);

            for (int i = 0; i < spawnCount; i++)
            {
                lock (enemyListLock)
                {
                    if (activeEnemies.Count >= currentCap)
                        break;
                }

                SpawnEnemy(elapsed);
                yield return null;
            }

            float interval = Mathf.Max(0.05f, currentSpawnInterval - (elapsed / 60f) * spawnRateGrowth);
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnEnemy(float elapsed)
    {
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        List<GameObject> availableEnemies = GetAvailableEnemies(elapsed);
        if (availableEnemies.Count == 0) return;

        GameObject prefab = availableEnemies[Random.Range(0, availableEnemies.Count)];

        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        
        lock (enemyListLock)
        {
            activeEnemies.Add(enemy);
        }

        ScaleEnemyDifficulty(enemy, elapsed);
    }

    List<GameObject> GetAvailableEnemies(float elapsed)
    {
        if (elapsed < bossTime1)
            return stage1Enemies;
        else if (elapsed < bossTime2)
            return stage2Enemies;
        else if (elapsed < bossTime3)
            return stage3Enemies;
        else
            return stage4Enemies;
    }

    void ScaleEnemyDifficulty(GameObject enemy, float elapsed)
    {
        Health health = enemy.GetComponent<Health>();
        SimpleEnemy2Dir enemyScript = enemy.GetComponent<SimpleEnemy2Dir>();

        float minutes = elapsed / 60f;
        float healthMult = 1f + minutes * healthGrowthPerMinute;
        float damageMult = 1f + minutes * damageGrowthPerMinute;

        if (health != null)
        {
            health.maxHealth = Mathf.RoundToInt(health.maxHealth * healthMult);
            health.Restore();
        }

        if (enemyScript != null)
        {
            enemyScript.contactDamage = Mathf.RoundToInt(enemyScript.contactDamage * damageMult);
        }
    }

    int GetSpawnCount(float elapsed)
    {
        if (elapsed < bossTime1)
            return spawnsPerBurst;
        else if (elapsed < bossTime2)
            return spawnsPerBurst + 2;
        else if (elapsed < bossTime3)
            return spawnsPerBurst + 4;
        else
            return spawnsPerBurst + 6;
    }

    int GetCurrentEnemyCap(float elapsed)
    {
        float progress = elapsed / finalBossTime;
        return Mathf.RoundToInt(Mathf.Lerp(baseEnemyCap, maxEnemyCap, progress));
    }

    IEnumerator BossSpawner()
    {
        yield return new WaitForSeconds(bossTime1);
        isBossActive = true;
        yield return new WaitForSeconds(1f);
        SpawnBoss(miniBoss1Prefab, "Первый мини-босс");
        yield return new WaitForSeconds(3f);
        isBossActive = false;

        yield return new WaitForSeconds(bossTime2 - bossTime1);
        isBossActive = true;
        yield return new WaitForSeconds(1f);
        SpawnBoss(miniBoss2Prefab, "Второй мини-босс");
        yield return new WaitForSeconds(3f);
        isBossActive = false;

        yield return new WaitForSeconds(bossTime3 - bossTime2);
        isBossActive = true;
        yield return new WaitForSeconds(1f);
        SpawnBoss(miniBoss3Prefab, "Третий мини-босс");
        yield return new WaitForSeconds(3f);
        isBossActive = false;

        yield return new WaitForSeconds(finalBossTime - bossTime3);
        isBossActive = true;
        yield return new WaitForSeconds(1f);
        SpawnBoss(finalBossPrefab, "Главный босс");
    }

    void SpawnBoss(GameObject bossPrefab, string message)
    {
        if (bossPrefab == null || spawnPoints.Length == 0) return;
        AudioManager.Instance?.PlayBossSpawn();
        Debug.Log(message);
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        
        lock (enemyListLock)
        {
            activeEnemies.Add(boss);
        }
    }

    public float GetProgress()
    {
        float current = Time.time - startTime;
        return Mathf.Clamp01(current / totalGameTime);
    }

    public float GetCurrentTime()
    {
        return Mathf.Clamp(Time.time - startTime, 0, totalGameTime);
    }
}