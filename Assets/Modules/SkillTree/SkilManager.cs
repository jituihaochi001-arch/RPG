using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

public class SkilManager : MonoBehaviour
{
    public static SkilManager Instance;
    //管理各项技能升级时属性的变化

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
    }
    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }
    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        string skillName = slot.skillSO.skillName;
        
        int bonus = slot.GetCurrentBonus();  // 根据当前层级获取加成值
        switch (skillName)
        {
            case "最大生命值加成":
                StatsManager.Instance.UpdateStat(StatsManager.StatType.MaxHealth, bonus);
                break;
            case "攻击力提升":
                StatsManager.Instance.UpdateStat(StatsManager.StatType.Damage, bonus);
                break;
            case "防御力提升":
                StatsManager.Instance.UpdateStat(StatsManager.StatType.Defense, bonus);
                break;
            case "魔力提升":
                StatsManager.Instance.UpdateStat(StatsManager.StatType.Magic, bonus);
                break;
            case "魔防提升":
                StatsManager.Instance.UpdateStat(StatsManager.StatType.MagicDefense, bonus);
                break;
            case "精神提升":
                StatsManager.Instance.UpdateStat(StatsManager.StatType.Spirit, bonus);
                break;
            default:
                Debug.LogWarning("未知技能: " + skillName);
                break;
        }

        // 技能加点后统一通知生命值 UI 刷新，避免最大生命值变化后 HUD 不更新
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.RefreshHealthUI();
        }
    }
}
