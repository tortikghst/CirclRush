using UnityEngine;

public class RunReward : MonoBehaviour
{
    public static RunReward Instance { get; private set; }

    public float loseRewardPercent = 0.25f;
    public float winRewardPercent = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("RunReward: создан");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Debug.Log("RunReward: OnEnable - подписываюсь");
        
        // Подписываемся на события
        Health.OnAnyBossDied += HandleWin;
        
        // Находим игрока и подписываемся на его смерть
        FindPlayerAndSubscribe();
    }

    void FindPlayerAndSubscribe()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDie.AddListener(HandleLose);
                Debug.Log("RunReward: подписан на смерть игрока");
            }
            else
            {
                Debug.LogError("RunReward: у игрока нет Health!");
            }
        }
        else
        {
            Debug.LogWarning("RunReward: игрок не найден, попробую позже");
            // Попробуем ещё раз через секунду
            Invoke(nameof(FindPlayerAndSubscribe), 0.5f);
        }
    }

    void OnDisable()
    {
        Debug.Log("RunReward: OnDisable - отписываюсь");
        Health.OnAnyBossDied -= HandleWin;
        
        // Отписываемся от игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDie.RemoveListener(HandleLose);
            }
        }
    }

    void HandleWin()
    {
        Debug.Log("RunReward: ПОБЕДА!");
        
        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance == null");
            return;
        }
        if (GoldManager.Instance == null)
        {
            Debug.LogError("GoldManager.Instance == null");
            return;
        }

        int coins = CoinManager.Instance.GetTotalCoinsCollectedThisRun();
        int reward = Mathf.RoundToInt(coins * winRewardPercent);
        Debug.Log($"Победа! coins={coins}, reward={reward}");
        GoldManager.Instance.AddGold(reward);
    }

    void HandleLose()
    {
        Debug.Log("RunReward: ПОРАЖЕНИЕ!");
        
        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance == null");
            return;
        }
        if (GoldManager.Instance == null)
        {
            Debug.LogError("GoldManager.Instance == null");
            return;
        }

        int coins = CoinManager.Instance.GetTotalCoinsCollectedThisRun();
        int reward = Mathf.RoundToInt(coins * loseRewardPercent);
        Debug.Log($"Поражение! coins={coins}, reward={reward}");
        GoldManager.Instance.AddGold(reward);
    }
}