using UnityEngine;
using System.Collections;

public class SimpleEnemy2Dir : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 3f;
    public float stopDistance = 0.5f;

    [Header("Боевые параметры")]
    public int contactDamage = 1;
    public float damageCooldown = 0.5f;

    [Header("Анимации")]
    public string animDownRight = "DownRight";
    public string animUpLeft = "UpLeft";

    [Header("Выпадение")]
    public GameObject expOrbPrefab;

    [Header("Визуальные эффекты")]
    public Color hitColor = Color.red;
    public float hitFlashDuration = 0.1f;

    [Header("Оптимизация (Culling)")]
    public bool useHitboxCulling = true;
    public float cullDistance = 20f;        // расстояние отключения хитбокса
    public float reactivateDistance = 18f;   // расстояние включения хитбокса
    public float cullingCheckInterval = 0.5f;

    // Приватные поля
    private static Transform _player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Health health;
    private Collider2D[] colliders;
    private float lastDamageTime;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private bool isDying = false;
    private bool isInvulnerable = false;     // неуязвим (хитбокс отключён)
    private float nextCullingCheck;

    // Кэшированные хэши анимаций
    private int animDownRightHash;
    private int animUpLeftHash;

    void Start()
    {
        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _player = playerObj.transform;
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        
        // Получаем все коллайдеры
        colliders = GetComponents<Collider2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        animDownRightHash = Animator.StringToHash(animDownRight);
        animUpLeftHash = Animator.StringToHash(animUpLeft);

        if (health != null)
        {
            health.OnDie.AddListener(OnDie);
            health.OnTakeDamage.AddListener(OnTakeDamage);
        }

        nextCullingCheck = Time.time + Random.Range(0f, 0.3f);
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDie.RemoveListener(OnDie);
            health.OnTakeDamage.RemoveListener(OnTakeDamage);
        }
    }

    void OnTakeDamage()
    {
        if (isDying || isInvulnerable) return; // не реагируем, если неуязвим
        
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
            
        flashCoroutine = StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitFlashDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
        
        flashCoroutine = null;
    }

    void Update()
    {
        if (_player == null || health == null || isDying) return;

        // Проверка расстояния для отключения хитбокса
        if (useHitboxCulling && Time.time >= nextCullingCheck)
        {
            nextCullingCheck = Time.time + cullingCheckInterval;
            CheckHitboxCulling();
        }

        MoveTowardsPlayer();
    }

    void CheckHitboxCulling()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (!isInvulnerable && distance > cullDistance)
        {
            SetInvulnerable(true);
        }
        else if (isInvulnerable && distance < reactivateDistance)
        {
            SetInvulnerable(false);
        }
    }

    void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;

        // Отключаем/включаем коллайдеры (хитбоксы)
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = !invulnerable;
        }

        // Визуальный эффект "неуязвимости" (опционально)
        if (spriteRenderer != null)
        {
            if (invulnerable)
                spriteRenderer.color = new Color(1, 1, 1, 0.5f); // полупрозрачный
            else
                spriteRenderer.color = originalColor;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = _player.position - transform.position;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            direction.Normalize();
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            UpdateAnimation(direction);
        }
    }

    void UpdateAnimation(Vector2 direction)
    {
        if (animator == null || spriteRenderer == null) return;

        direction.Normalize();
        
        float threshold = 0.3f;
        bool isMostlyHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) + threshold;
        bool isMostlyVertical = Mathf.Abs(direction.y) > Mathf.Abs(direction.x) + threshold;
        
        if (isMostlyHorizontal)
        {
            animator.Play(animDownRightHash);
            spriteRenderer.flipX = direction.x < 0;
        }
        else if (isMostlyVertical)
        {
            if (direction.y > 0)
            {
                animator.Play(animUpLeftHash);
                spriteRenderer.flipX = false;
            }
            else
            {
                animator.Play(animDownRightHash);
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                animator.Play(animDownRightHash);
                spriteRenderer.flipX = direction.x < 0;
            }
            else
            {
                if (direction.y > 0)
                {
                    animator.Play(animUpLeftHash);
                    spriteRenderer.flipX = false;
                }
                else
                {
                    animator.Play(animDownRightHash);
                    spriteRenderer.flipX = false;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDying || isInvulnerable) return; // не наносим урон, если неуязвим
        if (!collision.gameObject.CompareTag("Player")) return;
        
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                lastDamageTime = Time.time;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDying || isInvulnerable) return; // не наносим урон, если неуязвим
        if (!collision.gameObject.CompareTag("Player")) return;
        
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                lastDamageTime = Time.time;
            }
        }
    }

    void OnDie()
    {
        isDying = true;

        if (expOrbPrefab != null)
        {
            Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (useHitboxCulling)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, cullDistance);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, reactivateDistance);
        }
    }
}