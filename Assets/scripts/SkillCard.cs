using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText; // для отображения уровня

    private CardOption cardOption;
    private SkillSelectionUI selectionUI;

    public void Setup(CardOption option, SkillSelectionUI ui)
    {
        cardOption = option;
        selectionUI = ui;

        if (iconImage != null)
            iconImage.sprite = option.skillData.cardIcon;
        
        if (nameText != null)
            nameText.text = option.displayName;
        
        if (descriptionText != null)
            descriptionText.text = option.description;

        if (levelText != null)
        {
            if (option.type == CardType.NewSkill)
                levelText.text = "Новый";
            else
                levelText.text = $"Ур. {option.upgradeLevel}";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selectionUI?.OnOptionSelected(cardOption);
    }
}