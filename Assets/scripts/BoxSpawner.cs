using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Header("Префаб ящика")]
    public GameObject boxPrefab;

    [Header("Точки спавна")]
    public Transform[] spawnPoints;

    [Header("Настройки респавна")]
    public float respawnTime = 40f;

    private List<GameObject> activeBoxes = new List<GameObject>();
    private Dictionary<Vector3, float> respawnTimers = new Dictionary<Vector3, float>();

    void Start()
    {
        foreach (Transform point in spawnPoints)
        {
            SpawnBox(point.position);
        }
    }

    void Update()
    {
        // Создаём копию словаря для безопасного перебора
        Dictionary<Vector3, float> timersCopy = new Dictionary<Vector3, float>(respawnTimers);
        
        List<Vector3> positionsToRespawn = new List<Vector3>();
        
        // Перебираем копию
        foreach (var kvp in timersCopy)
        {
            // Обновляем значение в оригинальном словаре
            respawnTimers[kvp.Key] = kvp.Value - Time.deltaTime;
            
            if (respawnTimers[kvp.Key] <= 0)
            {
                positionsToRespawn.Add(kvp.Key);
            }
        }

        // Респавним ящики
        foreach (Vector3 pos in positionsToRespawn)
        {
            respawnTimers.Remove(pos);
            SpawnBox(pos);
        }
    }

    void SpawnBox(Vector3 position)
    {
        GameObject boxObj = Instantiate(boxPrefab, position, Quaternion.identity);
        Box box = boxObj.GetComponent<Box>();
        
        if (box != null)
        {
            box.OnBoxDestroyed += OnBoxDestroyed;
            activeBoxes.Add(boxObj);
        }
    }

    void OnBoxDestroyed(Vector3 position)
    {
        activeBoxes.RemoveAll(b => b == null);
        
        if (!respawnTimers.ContainsKey(position))
        {
            respawnTimers.Add(position, respawnTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                    Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }
}