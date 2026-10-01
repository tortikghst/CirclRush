using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour
{
    private Animator Animator;
    private Collider2D boxCollider;
    private Health health;

    [Header("Префабы лута")]
    public GameObject coin;
    public GameObject magnet;
    public GameObject potion;

    [Header("Настройки лута")]
    public float scatterRadius = 1.5f;
    public int minCoins = 3;
    public int maxCoins = 5;

    [Header("Анимация")]
    public float deathAnimationLength = 0.5f;

    [Header("События")]
    public System.Action<Vector3> OnBoxDestroyed;

    void Awake()
    {
        Animator = GetComponent<Animator>();
        boxCollider = GetComponent<Collider2D>();
        health = GetComponent<Health>();
        
        Debug.Log($"[Box] Awake: {gameObject.name}");
    }

    void Start()
    {
        if (health != null)
        {
            health.OnDie.AddListener(OnDie);
            health.autoDestroy = false;
            Debug.Log($"[Box] Start: подписался на OnDie");
        }
        else
        {
            Debug.LogError($"[Box] Start: Health компонент не найден!");
        }
    }

    void OnDie()
    {
        Debug.Log($"[Box] OnDie вызван для {gameObject.name}");
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        Debug.Log($"[Box] DieRoutine начата");
        
        // Звук разрушения ящика
        AudioManager.Instance?.PlayBoxBreak();
        
        // Запускаем анимацию смерти
        if (Animator != null)
        {
            Animator.SetBool("Death", true);
            Debug.Log($"[Box] Анимация смерти запущена");
        }
        else
        {
            Debug.LogError($"[Box] Animator не найден!");
        }

        // Отключаем коллайдер
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
            Debug.Log($"[Box] Коллайдер отключён");
        }

        // Ждём окончания анимации
        Debug.Log($"[Box] Ждём {deathAnimationLength} секунд");
        yield return new WaitForSeconds(deathAnimationLength);

        // Выбрасываем лут
        Debug.Log($"[Box] Вызываем Drop()");
        Drop();

        // Сообщаем спавнеру
        Debug.Log($"[Box] Вызываем OnBoxDestroyed");
        OnBoxDestroyed?.Invoke(transform.position);

        // Уничтожаем ящик
        Debug.Log($"[Box] Уничтожаем объект");
        Destroy(gameObject);
    }

    void Drop()
    {
        Debug.Log($"[Box] Drop() начат");
        
        if (coin == null) Debug.LogError("[Box] coin префаб не назначен!");
        if (magnet == null) Debug.LogError("[Box] magnet префаб не назначен!");
        if (potion == null) Debug.LogError("[Box] potion префаб не назначен!");

        // Монеты
        int coinCount = Random.Range(minCoins, maxCoins + 1);
        Debug.Log($"[Box] Создаём {coinCount} монет");
        
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * scatterRadius;
            randomOffset.z = 0;
            Instantiate(coin, transform.position + randomOffset, Quaternion.identity);
            Debug.Log($"[Box] Монета {i+1} создана");
        }
        
        // Магнит
        if (Random.value < 0.4f)
        {
            Debug.Log($"[Box] Создаём магнит");
            Vector3 randomOffset = Random.insideUnitSphere * scatterRadius;
            randomOffset.z = 0;
            Instantiate(magnet, transform.position + randomOffset, Quaternion.identity);
        }
        
        // Зелье
        if (Random.value < 0.3f)
        {
            Debug.Log($"[Box] Создаём зелье");
            Vector3 randomOffset = Random.insideUnitSphere * scatterRadius;
            randomOffset.z = 0;
            Instantiate(potion, transform.position + randomOffset, Quaternion.identity);
        }
        
        Debug.Log($"[Box] Drop() завершён");
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDie.RemoveListener(OnDie);
            Debug.Log($"[Box] OnDestroy: отписался от OnDie");
        }
    }
}