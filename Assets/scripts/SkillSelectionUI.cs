using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    [Header("Панель")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;
    [SerializeField] private List<SkillData> allSkills;
    [SerializeField] private int cardsToShow = 3;
    
    [Header("Кнопка замены")]
    [SerializeField] private Button replaceAllButton;
    [SerializeField] private TextMeshProUGUI swapCountText;
    
    private bool isOpen = false;

    private void Awake()
    {
        if (ExpManager.Instance != null)
        {
            ExpManager.Instance.OnLevelUpMenuRequest.AddListener(OnLevelUp);
        }
        else
        {
            Debug.LogError("ExpManager.Instance is null! Проверь порядок загрузки.");
        }

        selectionPanel.SetActive(false);
        
        if (replaceAllButton != null)
        {
            replaceAllButton.onClick.AddListener(OnReplaceAllClick);
        }
    }

    void Start()
    {
        RefreshSwapButton();
    }

    private void OnDestroy()
    {
        if (ExpManager.Instance != null)
            ExpManager.Instance.OnLevelUpMenuRequest.RemoveListener(OnLevelUp);
            
        if (replaceAllButton != null)
        {
            replaceAllButton.onClick.RemoveListener(OnReplaceAllClick);
        }
    }

    private void OnLevelUp(int newLevel)
    {
        Debug.Log($"OnLevelUp вызван для уровня {newLevel}, isOpen={isOpen}");
        if (isOpen) return;
        OpenSelection();
    }

    public void OpenSelection()
    {
        Debug.Log("OpenSelection()");
        selectionPanel.SetActive(true);
        isOpen = true;
        Time.timeScale = 0f;
        
        RefreshSwapButton();
        GenerateCards();
    }

    public void RefreshSwapButton()
    {
        int swapBonus = PlayerPrefs.GetInt("Bonus_SkillSwap", 0);
        Debug.Log($"Доступно замен: {swapBonus}");
        
        if (replaceAllButton != null)
        {
            replaceAllButton.interactable = swapBonus > 0;
        }
        
        if (swapCountText != null)
        {
            swapCountText.text = $"Замен: {swapBonus}";
        }
    }

    private void GenerateCards()
    {
        foreach (Transform child in cardsParent)
        {
            Destroy(child.gameObject);
        }

        List<CardOption> options = GenerateOptions();
        
        if (options.Count == 0)
        {
            Debug.Log("Нет доступных опций!");
            return;
        }
        
        List<CardOption> shuffled = options.OrderBy(x => Random.value).Take(cardsToShow).ToList();

        Debug.Log($"Выбрано {shuffled.Count} опций");

        foreach (CardOption option in shuffled)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardsParent);
            SkillCard card = cardObj.GetComponent<SkillCard>();
            if (card == null)
            {
                Debug.LogError("На префабе карточки нет компонента SkillCard!");
                continue;
            }
            card.Setup(option, this);
        }
    }

    private List<CardOption> GenerateOptions()
    {
        List<CardOption> options = new List<CardOption>();
        
        SkillManager sm = SkillManager.Instance;
        if (sm == null) return options;

        bool hasFreeSlot = sm.ActiveSkillsCount < sm.SkillSlots.Length;

        // Новые навыки (только если есть свободные слоты)
        if (hasFreeSlot)
        {
            List<SkillData> newSkills = allSkills.Where(s => !sm.HasSkill(s)).ToList();
            foreach (SkillData skill in newSkills)
            {
                options.Add(new CardOption 
                { 
                    type = CardType.NewSkill,
                    skillData = skill,
                    upgradeLevel = 1,
                    displayName = skill.skillName,
                    description = skill.description
                });
            }
        }

        // Улучшения для уже имеющихся навыков
        foreach (SkillInstance instance in sm.ActiveSkills)
        {
            if (instance.currentLevel < 10)
            {
                SkillUpgrade upgrade = instance.data.upgrades[instance.currentLevel - 1];
                
                options.Add(new CardOption
                {
                    type = CardType.Upgrade,
                    skillData = instance.data,
                    upgradeLevel = instance.currentLevel + 1,
                    displayName = $"{instance.data.skillName} +{instance.currentLevel + 1}",
                    description = upgrade?.upgradeDescription ?? "Улучшение навыка"
                });
            }
        }

        // Карточка замены (если есть бонус)
        int swapBonus = PlayerPrefs.GetInt("Bonus_SkillSwap", 0);
        if (swapBonus > 0)
        {
            options.Add(new CardOption
            {
                type = CardType.Swap,
                skillData = null,
                upgradeLevel = 0,
                displayName = "ЗАМЕНА ВСЕХ СКИЛОВ",
                description = $"Заменить все навыки. Осталось: {swapBonus}"
            });
        }

        return options.OrderBy(x => Random.value).ToList();
    }

    public void OnOptionSelected(CardOption option)
    {
        Debug.Log($"Выбрана опция: {option.displayName}");

        if (option.type == CardType.Swap)
        {
            OnReplaceAllClick();
            return;
        }

        AudioManager.Instance?.PlayButtonClick();
        
        SkillManager.Instance.AddOrUpgradeSkill(option.skillData);
        
        selectionPanel.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
    }
    
    public void OnReplaceAllClick()
    {
        if (SkillManager.Instance != null)
        {
            int swapBonus = PlayerPrefs.GetInt("Bonus_SkillSwap", 0);
            if (swapBonus <= 0)
            {
                Debug.Log("Нет доступных замен!");
                AudioManager.Instance?.PlayError();
                return;
            }
            
            bool success = SkillManager.Instance.ClearAllSkills();
            if (success)
            {
                Debug.Log("Все скилы удалены! Выберите новые.");
                AudioManager.Instance?.PlayPurchase();
                
                int newBonus = swapBonus - 1;
                PlayerPrefs.SetInt("Bonus_SkillSwap", newBonus);
                PlayerPrefs.Save();
                Debug.Log($"Осталось замен: {newBonus}");
                
                RefreshSwapButton();
                GenerateCards();
            }
            else
            {
                Debug.Log("Не удалось очистить скилы!");
                AudioManager.Instance?.PlayError();
            }
        }
    }
}

[System.Serializable]
public class CardOption
{
    public CardType type;
    public SkillData skillData;
    public int upgradeLevel;
    public string displayName;
    public string description;
}

public enum CardType
{
    NewSkill,
    Upgrade,
    Swap
}