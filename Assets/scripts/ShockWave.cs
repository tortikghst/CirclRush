using UnityEngine;
using System.Collections.Generic;

public class ShockWave : MonoBehaviour
{
    [Header("Настройки")]
    public int damage = 10;
    public float pushForce = 10f;

    private Animator animator;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private bool hasPlayedSound = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Destroy(gameObject);
            return;
        }

        // Звук при запуске волны
        AudioManager.Instance?.PlayShockwave();
        hasPlayedSound = true;

        float animLength = GetCurrentAnimationLength();
        Destroy(gameObject, animLength);
    }

    float GetCurrentAnimationLength()
    {
        if (animator.runtimeAnimatorController == null) return 1f;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == stateInfo.shortNameHash.ToString()) 
            {
                return clip.length;
            }
        }
        return 1f;
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
            
            Vector2 dir = (other.transform.position - transform.position).normalized;
            other.transform.position += (Vector3)dir * pushForce * 0.1f;
        }
    }
}