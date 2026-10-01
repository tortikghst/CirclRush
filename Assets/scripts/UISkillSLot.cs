using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillSlot : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI levelText; // новый текст для уровня
    [SerializeField] private Sprite emptyIcon;
    
    private SkillInstance currentSkill;

    public void SetSkillInstance(SkillInstance skill)
    {
        currentSkill = skill;
        if (skill != null && skill.data != null && skill.data.slotIcon != null)
        {
            iconImage.sprite = skill.data.slotIcon;
            iconImage.enabled = true;
            
            if (levelText != null)
                levelText.text = $"Lv.{skill.currentLevel}";
        }
        else
        {
            ClearSlot();
        }
    }

    private void Update()
    {
        if (currentSkill == null) return;

        if (currentSkill.currentCooldown > 0)
        {
            if (cooldownOverlay != null)
            {
                cooldownOverlay.gameObject.SetActive(true);
                cooldownOverlay.fillAmount = currentSkill.GetCooldownPercent();
            }
            
            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.Ceil(currentSkill.currentCooldown).ToString();
            }
        }
        else
        {
            if (cooldownOverlay != null)
                cooldownOverlay.gameObject.SetActive(false);
            
            if (cooldownText != null)
                cooldownText.gameObject.SetActive(false);
        }
    }

    public void ClearSlot()
    {
        currentSkill = null;
        
        if (emptyIcon != null)
        {
            iconImage.sprite = emptyIcon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
        
        if (cooldownOverlay != null)
            cooldownOverlay.gameObject.SetActive(false);
        
        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);
        
        if (levelText != null)
            levelText.text = "";
    }
}