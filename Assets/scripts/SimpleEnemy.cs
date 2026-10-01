using UnityEngine;
using System.Collections;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 3f;
    public float stopDistance = 0.5f;

    [Header("Боевые параметры")]
    public int contactDamage = 1;
    public float damageCooldown = 0.5f;

    [Header("Анимации")]
    public string animUp = "Up";
    public string animDown = "Down";
    public string animLeft = "Left";
    public string animRight = "Left"; // используем ту же анимацию, но с отражением

    [Header("Выпадение")]
    public GameObject expOrbPrefab;

    [Header("Визуальные эффекты")]
    public Color hitColor = Color.red;           // цвет при получении урона
    public float hitFlashDuration = 0.1f;         // длительность покраснения

    // Приватные поля
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Health health;
    private Rigidbody2D rb;
    private float lastDamageTime;
    private Vector2 lastDirection;
    private string currentAnimation;
    private float animationChangeTimer = 0f;
    private Vector2 stableDirection;
    private string targetAnimation = "";
    private bool targetFlip = false;
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("Стабильность анимаций")]
    public float animationStabilityTime = 0.2f;
    public float directionThreshold = 0.3f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("SimpleEnemy: Player not found! Ensure player has tag 'Player'.");

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Подписка на события Health
        if (health != null)
        {
            health.OnDie.AddListener(OnDie);
            health.OnTakeDamage.AddListener(OnTakeDamage);
        }
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDie.RemoveListener(OnDie);
            health.OnTakeDamage.RemoveListener(OnTakeDamage);
        }
    }

    // Вызывается при получении урона
    void OnTakeDamage()
    {
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
    }

    void FixedUpdate()
    {
        if (player == null || health == null) return;
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            direction.Normalize();
            rb.linearVelocity = direction * moveSpeed;
            UpdateAnimation(direction);
            lastDirection = direction;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (lastDirection != Vector2.zero)
                UpdateAnimation(lastDirection);
        }
    }

    void UpdateAnimation(Vector2 direction)
    {
        if (animator == null || spriteRenderer == null) return;
        
        if (direction.magnitude < 0.01f) return;

        direction.Normalize();

        string desiredAnimation = "";
        bool desiredFlip = false;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) + directionThreshold)
        {
            desiredAnimation = direction.x > 0 ? "Right" : "Left";
            desiredFlip = false;
        }
        else if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) + directionThreshold)
        {
            desiredAnimation = direction.y > 0 ? "Up" : "Down";
            desiredFlip = false;
        }
        else
        {
            return;
        }

        if (desiredAnimation != targetAnimation || desiredFlip != targetFlip)
        {
            targetAnimation = desiredAnimation;
            targetFlip = desiredFlip;
            animationChangeTimer = 0f;
        }
        else
        {
            animationChangeTimer += Time.deltaTime;
        }

       if (animationChangeTimer >= animationStabilityTime)
{
    // Получаем текущее состояние аниматора
    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    bool isAlreadyPlaying = false;

    // Проверяем, не проигрывается ли уже нужная анимация
    if (targetAnimation == "Left" && stateInfo.IsName("Left")) isAlreadyPlaying = true;
    else if (targetAnimation == "Right" && stateInfo.IsName("Left") && spriteRenderer.flipX == true) isAlreadyPlaying = true;
    else if (targetAnimation == "Up" && stateInfo.IsName("Up")) isAlreadyPlaying = true;
    else if (targetAnimation == "Down" && stateInfo.IsName("Down")) isAlreadyPlaying = true;

    if (!isAlreadyPlaying)
    {
        // Сбрасываем все параметры
        animator.SetBool("Up", false);
        animator.SetBool("Down", false);
        animator.SetBool("Left", false);

        // Устанавливаем нужный
        if (targetAnimation == "Right")
        {
            animator.SetBool("Left", true);
            spriteRenderer.flipX = true;
        }
        else if (targetAnimation == "Left")
        {
            animator.SetBool("Left", true);
            spriteRenderer.flipX = false;
        }
        else if (targetAnimation == "Up")
        {
            animator.SetBool("Up", true);
            spriteRenderer.flipX = false;
        }
        else if (targetAnimation == "Down")
        {
            animator.SetBool("Down", true);
            spriteRenderer.flipX = false;
        }
    }
}
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
    }

    void OnDie()
    {
        if (expOrbPrefab != null)
        {
            Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
        }
        
        // Уничтожаем врага сразу (без анимации смерти)
        Destroy(gameObject);
    }
}