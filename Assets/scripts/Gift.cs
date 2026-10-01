using UnityEngine;
using System.Collections;

public class Gift : MonoBehaviour
{
    [Header("Настройки")]
    public float explosionRadius = 4.5f;
    public int explosionDamage = 20;
    public float fallDuration = 1.017f;       // Bomb_fly
    public float readyDuration = 1.017f;      // Bomb_ready
    public float explodeDuration = 1.017f;    // Bomb_explou
    public float startHeight = 8f;             // высота появления

    [Header("Анимации")]
    public string flyAnimation = "Bomb_fly";
    public string readyAnimation = "Bomb_ready";
    public string explodeAnimation = "Bomb_explou";

    private Animator animator;
    private Vector3 targetPosition;
    private bool isExploding = false;

    public void Initialize(Vector3 landPos)
    {
        targetPosition = landPos;
        transform.position = new Vector3(landPos.x, landPos.y + startHeight, landPos.z);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Gift: Animator не найден!");
            Destroy(gameObject);
            return;
        }

        animator.Play(flyAnimation);
        StartCoroutine(FallToGround());
    }

    IEnumerator FallToGround()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;
            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        StartReady();
    }

    void StartReady()
    {
        animator.Play(readyAnimation);
        Invoke(nameof(Explode), readyDuration);
    }

    void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        animator.Play(explodeAnimation);
        AudioManager.Instance?.PlayGiftDrop();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, SkillManager.Instance.enemyLayer);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(explosionDamage, gameObject);
        }

        Destroy(gameObject, explodeDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}