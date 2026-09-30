using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillSlot : MonoBehaviour
{
    public List<SkillSlot> prerequisiteSkillSLots;
    public SkillSO skillSO;

    public TMP_Text skillLeveltext;
    
    public int currentTier;          // 当前层级 (0-3)
    public int currentLevelInTier;   // 当前层级内的等级 (0-5)

    public bool isUnlocked;

    public Image skillIcon;
    public Button skillButton;
    //声明全局事件：技能点消耗
    public static event Action <SkillSlot> OnAbilityPointSpent;
    public static event Action<SkillSlot> OnSkillMaxed;

    private void OnValidate()
    {
        if(skillSO != null && skillLeveltext != null)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (skillSO == null)
        {
            Debug.LogWarning($"SkillSlot {gameObject.name} 的 skillSO 为空！");
            return;
        }
        skillIcon.sprite = skillSO.skillIcon;
        if (isUnlocked)
        {
            skillButton.interactable = true;
            skillLeveltext.text = currentLevelInTier.ToString() + "/" + skillSO.maxLevel.ToString();
            skillIcon.color = Color.white;
        }
        else
        {
            skillButton.interactable = false;
            skillIcon.color = Color.grey;
            skillLeveltext.text = "Locked";
        }
    }

    // ===== 获取当前等级加成值 =====
    public int GetCurrentBonus()
    {
        if (skillSO == null || currentLevelInTier == 0) return 0;
        if (skillSO.tiers == null || currentTier >= skillSO.tiers.Length) return 0;

        return skillSO.tiers[currentTier].bonusPerLevel;
    }
    // ===== 获取当前总等级（用于判断是否满级） =====
    public int GetTotalLevel()
    {
        return currentTier * 5 + currentLevelInTier;
    }

    // ===== 是否已满级（4层全满） =====
    public bool IsMaxLevel()
    {
        return currentTier >= 3 && currentLevelInTier >= 5;
    }

    //升级技能
    public void TryUpgradeSkill()
    {
        if (!isUnlocked) return;
        if (IsMaxLevel()) return;
        // 当前层级等级 +1
        currentLevelInTier++;
        UpdateUI();
        OnAbilityPointSpent?.Invoke(this);
        if (currentLevelInTier >= 5)
        {        
            // 触发满级事件
            OnSkillMaxed?.Invoke(this);
            return;
        }       
        UpdateUI();
    }
    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisiteSkillSLots) 
        {
            if (!slot.isUnlocked || slot.currentLevelInTier < slot.skillSO.maxLevel) 
            { 
                return false;
            }
        }
        return true;
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateUI();
    }
  
}
