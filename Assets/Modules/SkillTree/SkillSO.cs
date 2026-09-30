using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewSkill",menuName = "SkillTree")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;

    // 每层配置：4 层，每层 5 级
    public SkillTier[] tiers = new SkillTier[4];

    public int maxLevel = 5;      // 1层有5级
}

[System.Serializable]
public class SkillTier
{
    public int bonusPerLevel;       // 每级加成值    
}

