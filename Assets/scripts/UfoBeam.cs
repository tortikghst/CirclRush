using UnityEngine;
using System.Collections;

public class UfoBeam : MonoBehaviour
{
    [Header("Настройки")]
    public float duration = 4f;
    public float tickInterval = 1f;
    public int damagePerTick = 10;
    public float liftHeight = 5.5f;

    [Header("Анимации")]
    public string beamAnimation = "beam";
    public float beamDuration = 1f;

    private Transform targetEnemy;
    private Rigidbody2D enemyRb;
    private Health enemyHealth;
    private MonoBehaviour[] enemyScripts;
    private Collider2D enemyCollider;
    private Vector3 originalPosition;
    private bool isActive = true;
    private bool enemyDead = false;

    public void Initialize(Transform enemy)
    {
        targetEnemy = enemy;
    }

    void Start()
    {
        if (targetEnemy == null)
        {
            Destroy(gameObject);
            return;
        }
        AudioManager.Instance?.PlayUfoBeam(); // <-- добавить
        enemyRb = targetEnemy.GetComponent<Rigidbody2D>();
        enemyHealth = targetEnemy.GetComponent<Health>();
        enemyCollider = targetEnemy.GetComponent<Collider2D>();
        enemyScripts = targetEnemy.GetComponents<MonoBehaviour>();

        originalPosition = targetEnemy.position;

        if (enemyRb != null)
        {
            enemyRb.simulated = false;
            enemyRb.linearVelocity = Vector2.zero;
            enemyRb.angularVelocity = 0f;
        }

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        foreach (var script in enemyScripts)
        {
            if (script != null && script != enemyHealth)
                script.enabled = false;
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null && !string.IsNullOrEmpty(beamAnimation))
            anim.Play(beamAnimation);

        StartCoroutine(LiftEnemy());
        InvokeRepeating(nameof(DealDamage), 0f, tickInterval);
        Invoke(nameof(Finish), duration);
    }

    IEnumerator LiftEnemy()
    {
        float elapsed = 0f;
        float liftTime = 0.3f;
        
        // Сохраняем начальную позицию (если враг ещё жив)
        if (targetEnemy == null) yield break;
        Vector3 startPos = targetEnemy.position;
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + liftHeight, startPos.z);

        while (elapsed < liftTime)
        {
            // Проверяем, жив ли враг
            if (targetEnemy == null) yield break;
            
            elapsed += Time.deltaTime;
            float t = elapsed / liftTime;
            targetEnemy.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        if (targetEnemy != null)
            targetEnemy.position = targetPos;
    }

    IEnumerator LowerEnemy()
    {
        float elapsed = 0f;
        float lowerTime = 0.5f;
        
        if (targetEnemy == null) yield break;
        Vector3 startPos = targetEnemy.position;
        Vector3 targetPos = originalPosition;

        while (elapsed < lowerTime)
        {
            if (targetEnemy == null) yield break;
            
            elapsed += Time.deltaTime;
            float t = elapsed / lowerTime;
            targetEnemy.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        if (targetEnemy != null)
            targetEnemy.position = targetPos;
    }

    void DealDamage()
    {
        if (!isActive || targetEnemy == null || enemyHealth == null) return;

        enemyHealth.TakeDamage(damagePerTick, gameObject);

        if (targetEnemy == null)
        {
            enemyDead = true;
        }
    }

    void Finish()
    {
        isActive = false;
        CancelInvoke();

        if (targetEnemy != null && !enemyDead)
        {
            StartCoroutine(LowerEnemy());
            Invoke(nameof(Unfreeze), 0.6f);
        }
        else
        {
            Unfreeze();
        }
    }

    void Unfreeze()
    {
        if (targetEnemy != null)
        {
            if (enemyRb != null)
                enemyRb.simulated = true;

            if (enemyCollider != null)
                enemyCollider.enabled = true;

            foreach (var script in enemyScripts)
            {
                if (script != null && script != enemyHealth)
                    script.enabled = true;
            }
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (targetEnemy != null)
            transform.position = targetEnemy.position;
    }
}