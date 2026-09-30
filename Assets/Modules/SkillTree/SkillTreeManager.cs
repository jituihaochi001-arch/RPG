using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    public SkillSlot[] skillSlots;
    public TMP_Text pointsText;
    public int avaliablePoints;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        foreach (SkillSlot slot in skillSlots) 
        {
            slot.skillButton.onClick.AddListener(() => CheckAvaliablePoints(slot));
        }
        UpdateAbilityPoints(0);
    }

    private void CheckAvaliablePoints(SkillSlot slot)
    {
        if (avaliablePoints > 0 && slot.currentLevelInTier < 5)
        {
            slot.TryUpgradeSkill();
        }
    }
    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandeleAbilityPointsSpent;
        SkillSlot.OnSkillMaxed += HandeleSkillMaxed;
        ExpManager.OnLevelUp += UpdateAbilityPoints;
    }
    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandeleAbilityPointsSpent;
        SkillSlot.OnSkillMaxed -= HandeleSkillMaxed;
        ExpManager.OnLevelUp -= UpdateAbilityPoints;
    }
    //升级技能时消耗技能点
    private void HandeleAbilityPointsSpent(SkillSlot skillSlot)
    {
        if (avaliablePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }
    }
    //当技能点升满时
    private void HandeleSkillMaxed(SkillSlot skillSlot)
    {
        foreach(SkillSlot slot in skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
            {
                slot.Unlock(); 
            }
        }
    }
    //获得技能点
    public void UpdateAbilityPoints(int amount)
    {
        avaliablePoints += amount;
        pointsText.text = "Points : " + avaliablePoints;
    }
    // ===== 获取存档数据 =====
    public SkillTreeData GetSaveData()
    {
        SkillTreeData saveData = new SkillTreeData();
        saveData.availablePoints = avaliablePoints;

        foreach (var slot in skillSlots)
        {
            if (slot.skillSO != null)
            {
                SkillSlotData slotData = new SkillSlotData();
                slotData.skillName = slot.skillSO.skillName;
                slotData.currentTier = slot.currentTier;
                slotData.currentLevelInTier = slot.currentLevelInTier;
                slotData.isUnlocked = slot.isUnlocked;
                saveData.slotDataList.Add(slotData);
            }
        }

        return saveData;
    }

    // ===== 加载存档数据 =====
    public void LoadSaveData(SkillTreeData saveData)
    {
        if (saveData == null) return;

        // 恢复技能点
        avaliablePoints = saveData.availablePoints;
        UpdateAbilityPoints(0);  // 刷新 UI

        // 恢复每个技能槽
        foreach (var slotData in saveData.slotDataList)
        {
            // 根据技能名称查找对应的 SkillSlot
            SkillSlot targetSlot = FindSlotBySkillName(slotData.skillName);
            if (targetSlot != null)
            {
                targetSlot.currentTier = slotData.currentTier;
                targetSlot.currentLevelInTier = slotData.currentLevelInTier;
                targetSlot.isUnlocked = slotData.isUnlocked;
                targetSlot.UpdateUI();
            }
            else
            {
                Debug.LogWarning($"未找到技能: {slotData.skillName}");
            }
        }
    }

    // ===== 根据技能名称查找 Slot =====
    private SkillSlot FindSlotBySkillName(string skillName)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.skillSO != null && slot.skillSO.skillName == skillName)
            {
                return slot;
            }
        }
        return null;
    }
}
