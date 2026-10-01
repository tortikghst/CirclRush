using UnityEngine;

public class OrbScript : MonoBehaviour
{
    [Header("Настройки")]
    public float speed = 8f;
    public int damage = 10;        // будет меняться извне
    public float lifetime = 3f;
    
    [Header("Анимация")]
    public Animator animator;
    public string deathAnimationName = "explouse_orb";
    public float deathAnimationLength = 0.517f;
    
    private Transform target;
    private bool hit = false;
    private bool isDying = false;
    
    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Transform bulletTarget)
    {
        target = bulletTarget;
    }
    
    void Update()
    {
        if (isDying) return;
        if (hit) return;
        
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
        if (hit || isDying) return;
        
        if (target != null && other.transform == target)
        {
            HitTarget();
        }
    }
    
    void HitTarget()
    {
        if (hit || isDying) return;
        hit = true;
        
        if (target != null)
        {
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, gameObject);
            }
        }
        
        Die();
    }
    
    void Die()
    {
        if (isDying) return;
        isDying = true;
        
        speed = 0;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        if (animator != null)
        {
            animator.Play(deathAnimationName, 0, 0f);
        }
        
        CancelInvoke();
        Invoke(nameof(DestroyNow), deathAnimationLength);
    }
    
    void DestroyNow()
    {
        Destroy(gameObject);
    }
}