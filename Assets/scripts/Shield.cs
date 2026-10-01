using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    [Header("Настройки")]
    public float duration = 5f;
    public float speedMultiplier = 1.5f;
    public Vector3 offset = new Vector3(0, -1.2f, 0); // смещение вниз (по Y)

    [Header("Визуал")]
    public GameObject shieldVisuals;
    public GameObject particlesParent;
    public float particleRotationSpeed = 100f;

    [Header("Анимации")]
    public string appearAnimation = "ShieldAppear";
    public string idleAnimation = "ShieldIdle";
    public string disappearAnimation = "ShieldDisappear";
    public float appearDuration = 0.5f;
    public float disappearDuration = 0.5f;

    private PlayerBehavour playerMovement;
    private Health playerHealth;
    private Animator animator;
    private float originalSpeed;
    private bool isActive = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Shield: Player not found!");
            Destroy(gameObject);
            return;
        }

        playerMovement = player.GetComponent<PlayerBehavour>();
        playerHealth = player.GetComponent<Health>();
        animator = GetComponent<Animator>();

        if (playerMovement == null)
        {
            Debug.LogError("Shield: PlayerMovement not found!");
            Destroy(gameObject);
            return;
        }

        originalSpeed = playerMovement.moveSpeed;

        // Прикрепляем к игроку
        transform.SetParent(player.transform);
        
        // Устанавливаем смещение
        transform.localPosition = offset;

        // По умолчанию визуал выключен
        if (shieldVisuals != null)
            shieldVisuals.SetActive(false);

        StartCoroutine(AppearRoutine());
    }

    IEnumerator AppearRoutine()
    {
        if (shieldVisuals != null)
            shieldVisuals.SetActive(true);

        if (animator != null && !string.IsNullOrEmpty(appearAnimation))
        {
            animator.Play(appearAnimation);
        }

        yield return new WaitForSeconds(appearDuration);

        ActivateEffects();

        if (animator != null && !string.IsNullOrEmpty(idleAnimation))
        {
            animator.Play(idleAnimation);
        }

        Invoke(nameof(StartDisappear), duration);
    }

    void ActivateEffects()
{
    isActive = true;
    AudioManager.Instance?.PlayShieldActivate(); // <-- добавить
    playerMovement.moveSpeed = originalSpeed * speedMultiplier;
    
    if (playerHealth != null)
    {
        playerHealth.damageResist = 0f;
    }
}

    void Update()
    {
        if (isActive && particlesParent != null)
        {
            particlesParent.transform.Rotate(0, 0, particleRotationSpeed * Time.deltaTime);
        }
    }

    void StartDisappear()
    {
        if (!isActive) return;
        StartCoroutine(DisappearRoutine());
    }

    IEnumerator DisappearRoutine()
    {
        isActive = false;
        
        playerMovement.moveSpeed = originalSpeed;
        if (playerHealth != null)
        {
            playerHealth.damageResist = 1f;
        }

        if (animator != null && !string.IsNullOrEmpty(disappearAnimation))
        {
            animator.Play(disappearAnimation);
        }

        yield return new WaitForSeconds(disappearDuration);

        if (shieldVisuals != null)
            shieldVisuals.SetActive(false);

        Destroy(gameObject, 0.1f);
    }

    void OnDestroy()
    {
        if (isActive && playerMovement != null)
        {
            playerMovement.moveSpeed = originalSpeed;
            if (playerHealth != null)
                playerHealth.damageResist = 1f;
        }
    }
}