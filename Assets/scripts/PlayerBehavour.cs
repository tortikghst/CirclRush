using UnityEngine;

public class PlayerBehavour : MonoBehaviour, IDamagiable
{
    public static PlayerBehavour Instance { get; private set; }

    [Header("Скорость")]
    public float moveSpeed;
    public GameObject Player;
    private Rigidbody2D plrb;
    private Animator Animator;
    private Health health;

    [Header("Здоровье")]
    public float hp { get; set; }
    public float currentHp { get; set; }
    public float damageResist { get; set; }
    
    public float LastMoveX { get; private set; }
    public float LastMoveY { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        plrb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        hp = currentHp;

        RefreshSpeed();

        if (health != null)
        {
            health.OnDie.AddListener(OnPlayerDeath);
        }
    }

    public void RefreshSpeed()
    {
        float bonus = PlayerPrefs.GetFloat("Bonus_MoveSpeed", 0f);
        moveSpeed = 5f + bonus;
        Debug.Log($"Скорость обновлена: {moveSpeed} (бонус: {bonus})");
    }

    public void ApplySpeedBonus()
    {
        RefreshSpeed();
    }

    void Update()
    {
        Moving();
        Animation();
    }

    void Moving()
    {
        float hMoving = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        float vMoving = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;
        plrb.linearVelocity = new Vector2(hMoving, vMoving).normalized * moveSpeed;
    }

    void Hit(float amount)
    {
        if (currentHp > 0)
            currentHp -= amount * damageResist;
        if (currentHp <= 0)
        {
            Animator.SetBool("Death", true);
            Destroy(Player);
        }
    }

    void OnPlayerDeath()
    {
        Debug.Log("Игрок умер - показываем меню поражения");
        FindObjectOfType<EndGameMenu>()?.ShowGameOver();
    }

    void Animation()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (moveInput.magnitude > 0.1f)
        {
            Animator.SetBool("idle", false);

            LastMoveX = moveInput.x;
            LastMoveY = moveInput.y;

            Animator.SetFloat("lastMoveX", moveInput.x);
            Animator.SetFloat("lastMoveY", moveInput.y);

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0)
                {
                    Animator.SetBool("goright", true);
                    Animator.SetBool("goleft", false);
                }
                else
                {
                    Animator.SetBool("goright", false);
                    Animator.SetBool("goleft", true);
                }
                Animator.SetBool("goup", false);
                Animator.SetBool("godown", false);
            }
            else
            {
                if (moveInput.y > 0)
                {
                    Animator.SetBool("goup", true);
                    Animator.SetBool("godown", false);
                }
                else
                {
                    Animator.SetBool("goup", false);
                    Animator.SetBool("godown", true);
                }
                Animator.SetBool("goright", false);
                Animator.SetBool("goleft", false);
            }
        }
        else
        {
            Animator.SetBool("idle", true);
            Animator.SetBool("goup", false);
            Animator.SetBool("godown", false);
            Animator.SetBool("goright", false);
            Animator.SetBool("goleft", false);
        }
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDie.RemoveListener(OnPlayerDeath);
        }
    }
}