using UnityEngine;
using System.Collections.Generic;

public class OrbitingDisk : MonoBehaviour
{
    [Header("Настройки")]
    public float rotationSpeed = 180f;
    public float orbitRadius = 3f;
    public float lifeTime = 5f;
    public int damage = 15;

    private Transform player;
    private float currentAngle;
    private float spawnTime;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private bool hasPlayedSpawnSound = false;

    public void Initialize(Transform playerTransform, float startAngle)
    {
        player = playerTransform;
        currentAngle = startAngle;
        spawnTime = Time.time;
        UpdatePosition();
        
        // Звук при появлении диска (только один раз)
        if (!hasPlayedSpawnSound)
        {
            AudioManager.Instance?.PlayDiskSpin();
            hasPlayedSpawnSound = true;
        }
    }

    void Update()
    {
        if (player == null) return;

        currentAngle += rotationSpeed * Time.deltaTime;
        UpdatePosition();

        if (Time.time - spawnTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void UpdatePosition()
    {
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * orbitRadius;
        transform.position = player.position + offset;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & SkillManager.Instance.enemyLayer) == 0)
            return;

        if (damagedEnemies.Contains(other.gameObject))
            return;

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage, gameObject);
            damagedEnemies.Add(other.gameObject);
        }
    }
}