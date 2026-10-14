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

    // ===== 技能树初始状态快照 =====
    // 新游戏时需要恢复到预制体配置的初始状态，而不是硬编码为 0：
    // 根节点的默认解锁状态、初始技能点等信息都可能由策划在预制体里配置。
    [System.Serializable]
    private class SkillSlotDefaultState
    {
        public SkillSlot slot;
        public int tier;
        public int levelInTier;
        public bool isUnlocked;
    }

    private readonly List<SkillSlotDefaultState> defaultStates = new List<SkillSlotDefaultState>();
    private int defaultAvailablePoints;
    private bool defaultStateCaptured;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 在没有任何加点、读档之前记录技能树的初始状态
            CaptureDefaultState();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        foreach (SkillSlot slot in skillSlots) 
        {
            if (slot == null || slot.skillButton == null) continue;
            slot.skillButton.onClick.AddListener(() => CheckAvaliablePoints(slot));
        }
        RefreshPointsText();
    }

    private void CheckAvaliablePoints(SkillSlot slot)
    {
        // 使用技能自身的最大等级，避免以后调整 maxLevel 时还要同步修改这里的数字
        if (slot != null && slot.skillSO != null &&
            avaliablePoints > 0 && slot.currentLevelInTier < slot.skillSO.maxLevel)
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
        RefreshPointsText();
    }

    // ===== 记录技能树初始状态（预制体配置的默认值）=====
    private void CaptureDefaultState()
    {
        defaultStates.Clear();
        defaultAvailablePoints = avaliablePoints;

        foreach (SkillSlot slot in skillSlots)
        {
            if (slot == null) continue;

            defaultStates.Add(new SkillSlotDefaultState
            {
                slot = slot,
                tier = slot.currentTier,
                levelInTier = slot.currentLevelInTier,
                isUnlocked = slot.isUnlocked
            });
        }

        defaultStateCaptured = true;
    }

    // ===== 恢复技能树初始状态（新游戏时调用）=====
    public void ResetToDefault()
    {
        if (!defaultStateCaptured)
        {
            CaptureDefaultState();
        }

        avaliablePoints = defaultAvailablePoints;

        foreach (SkillSlotDefaultState state in defaultStates)
        {
            if (state.slot == null) continue;

            state.slot.currentTier = state.tier;
            state.slot.currentLevelInTier = state.levelInTier;
            state.slot.isUnlocked = state.isUnlocked;
            state.slot.UpdateUI();
        }

        RefreshPointsText();
    }

    // ===== 获取存档数据 =====
    public SkillTreeData GetSaveData()
    {
        SkillTreeData saveData = new SkillTreeData();
        // 版本 2：通过 slotIndex 精确定位技能节点，解决多个节点复用同一个 SkillSO 时按名称串档的问题
        saveData.saveVersion = 2;
        saveData.availablePoints = avaliablePoints;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            SkillSlot slot = skillSlots[i];
            if (slot == null || slot.skillSO == null) continue;

            SkillSlotData slotData = new SkillSlotData();
            slotData.slotIndex = i;
            slotData.skillName = slot.skillSO.skillName;
            slotData.currentTier = slot.currentTier;
            slotData.currentLevelInTier = slot.currentLevelInTier;
            slotData.isUnlocked = slot.isUnlocked;
            saveData.slotDataList.Add(slotData);
        }

        return saveData;
    }

    // ===== 加载存档数据 =====
    public void LoadSaveData(SkillTreeData saveData)
    {
        if (saveData == null) return;

        // 先把整棵树恢复到初始状态，再覆盖存档中的数据：
        // 这样存档里没有记录的节点不会残留上一次运行或上一次读档的数值。
        ResetToDefault();

        // 恢复技能点
        avaliablePoints = saveData.availablePoints;
        RefreshPointsText();

        // 记录已经恢复过的节点，防止旧存档中的重复名称覆盖到同一个 SkillSlot
        HashSet<SkillSlot> restoredSlots = new HashSet<SkillSlot>();

        // 恢复每个技能槽
        foreach (var slotData in saveData.slotDataList)
        {
            SkillSlot targetSlot = null;

            // 新版本存档：slotIndex 是唯一且稳定的节点标识
            if (saveData.saveVersion >= 2 &&
                slotData.slotIndex >= 0 &&
                slotData.slotIndex < skillSlots.Length)
            {
                targetSlot = skillSlots[slotData.slotIndex];
            }

            // 旧版本存档没有 slotIndex，使用 skillName + currentTier 作为复合键查找。
            // 每个技能对应 4 个 tier 节点，该复合键在当前技能树结构中是唯一的。
            if (targetSlot == null)
            {
                targetSlot = FindSlotForLegacyData(slotData, restoredSlots);
            }

            if (targetSlot == null)
            {
                Debug.LogWarning($"技能树读档失败：未找到技能节点 {slotData.skillName} (Tier {slotData.currentTier})");
                continue;
            }

            if (!restoredSlots.Add(targetSlot))
            {
                Debug.LogWarning($"技能树读档：节点 {targetSlot.name} 被重复写入，已跳过重复数据");
                continue;
            }

            targetSlot.currentTier = slotData.currentTier;
            targetSlot.currentLevelInTier = slotData.currentLevelInTier;
            targetSlot.isUnlocked = slotData.isUnlocked;
            targetSlot.UpdateUI();
        }
    }

    // ===== 兼容旧存档：使用技能名 + 层级查找尚未恢复的节点 =====
    private SkillSlot FindSlotForLegacyData(SkillSlotData slotData, HashSet<SkillSlot> restoredSlots)
    {
        foreach (SkillSlot slot in skillSlots)
        {
            if (slot == null || slot.skillSO == null) continue;
            if (restoredSlots.Contains(slot)) continue;

            if (slot.skillSO.skillName == slotData.skillName && slot.currentTier == slotData.currentTier)
            {
                return slot;
            }
        }

        return null;
    }

    // ===== 刷新技能点 UI =====
    private void RefreshPointsText()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points : " + avaliablePoints;
        }
    }
}
