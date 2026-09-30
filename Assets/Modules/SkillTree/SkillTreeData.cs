using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===== 技能树存档数据结构 =====
[System.Serializable]
public class SkillTreeData
{
    public List<SkillSlotData> slotDataList = new List<SkillSlotData>();
    public int availablePoints;
}

[System.Serializable]
public class SkillSlotData
{
    public string skillName;              // 技能名称（用于查找对应的 SkillSO）
    public int currentTier;               // 当前层级
    public int currentLevelInTier;        // 当前层级内的等级
    public bool isUnlocked;               // 是否解锁
}
