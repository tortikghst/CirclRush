using UnityEngine;

public class Shadow : MonoBehaviour
{
    [Header("Настройки")]
    public float duration = 3.017f;      // Shadow_fly

    [Header("Анимации")]
    public string flyAnimation = "Shadow_fly";

    private Animator animator;

    public void Initialize(Vector3 landPos)
    {
        transform.position = landPos;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Shadow: Animator не найден!");
            Destroy(gameObject);
            return;
        }

        animator.Play(flyAnimation);
        Destroy(gameObject, duration);
    }
}