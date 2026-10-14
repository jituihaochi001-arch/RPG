using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===== 技能树存档数据结构 =====
[System.Serializable]
public class SkillTreeData
{
    // 存档版本号：0 表示旧存档，2 表示使用 slotIndex 精确定位技能节点的新存档
    public int saveVersion;
    public List<SkillSlotData> slotDataList = new List<SkillSlotData>();
    public int availablePoints;
}

[System.Serializable]
public class SkillSlotData
{
    public int slotIndex;                 // 技能节点在 SkillTreeManager.skillSlots 中的下标，用于唯一标识节点
    public string skillName;              // 技能名称（用于查找对应的 SkillSO）
    public int currentTier;               // 当前层级
    public int currentLevelInTier;        // 当前层级内的等级
    public bool isUnlocked;               // 是否解锁
}
