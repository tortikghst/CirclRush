using UnityEngine;
using System.Collections;

public class BouncingOrb : MonoBehaviour
{
    [Header("Настройки")]
    public float speed = 10f;
    public int damage = 5;
    public int maxBounces = 1;
    public float searchRadius = 20f; // радиус поиска новых целей
    
    [Header("Анимации")]
    public Animator animator;
    public string flyAnimation = "Fly";
    public string explodeAnimation = "Explode";
    public float explodeDuration = 0.5f;
    
    private Transform target;
    private int currentBounces = 0;
    private bool isDying = false;
    private bool isExploding = false;
    private bool hasPlayedLaunchSound = false;
    
    void Start()
    {
        if (animator != null && !string.IsNullOrEmpty(flyAnimation))
            animator.Play(flyAnimation);
        
        // Звук при запуске снаряда
        AudioManager.Instance?.PlayBouncingOrb();
        hasPlayedLaunchSound = true;
        
        Invoke(nameof(TimeOut), 5f);
    }
    
    void TimeOut()
    {
        if (!isDying && !isExploding) Die();
    }
    
    public void Initialize(Transform initialTarget)
    {
        target = initialTarget;
    }
    
    void Update()
    {
        if (isDying || isExploding) return;
        if (target == null)
        {
            Die();
            return;
        }
        
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        if (Vector2.Distance(transform.position, target.position) < 0.3f)
        {
            HitTarget();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying || isExploding) return;
        if (other.transform == target)
        {
            HitTarget();
        }
    }
    
    void HitTarget()
    {
        if (isDying || isExploding) return;
        
        // Звук при попадании (отскоке)
        AudioManager.Instance?.PlayBouncingOrb();
        
        Health health = target.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage, gameObject);
        
        StartCoroutine(HandleHit());
    }
    
    IEnumerator HandleHit()
    {
        isExploding = true;
        speed = 0;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        if (animator != null && !string.IsNullOrEmpty(explodeAnimation))
        {
            animator.Play(explodeAnimation);
        }
        
        yield return new WaitForSeconds(explodeDuration);
        
        if (currentBounces >= maxBounces)
        {
            Die();
            yield break;
        }
        
        Transform newTarget = FindNewTarget();
        if (newTarget != null)
        {
            target = newTarget;
            currentBounces++;
            
            if (animator != null && !string.IsNullOrEmpty(flyAnimation))
                animator.Play(flyAnimation);
            
            if (col != null) col.enabled = true;
            
            isExploding = false;
            speed = 10f;
        }
        else
        {
            Die();
        }
    }
    
    Transform FindNewTarget()
    {
        if (SkillManager.Instance != null)
        {
            return SkillManager.Instance.GetRandomEnemyInRange(transform.position, searchRadius, target);
        }
        return null;
    }
    
    void Die()
    {
        if (isDying) return;
        isDying = true;
        
        CancelInvoke();
        Destroy(gameObject);
    }
}