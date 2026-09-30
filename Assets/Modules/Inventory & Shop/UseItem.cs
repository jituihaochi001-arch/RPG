using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void ApplyItemEffects(ItemSO itemSO)
    {
        // 立即生效的属性
        if (itemSO.currentHealth != 0)
            StatsManager.Instance.ChangeHealth(itemSO.currentHealth);

        if (itemSO.maxHealth != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.MaxHealth, itemSO.maxHealth);

        if (itemSO.damage != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Damage, itemSO.damage);

        if (itemSO.magic != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Magic, itemSO.magic);

        if (itemSO.defense != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Defense, itemSO.defense);

        if (itemSO.magicDefense != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.MagicDefense, itemSO.magicDefense);

        if (itemSO.spirit != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Spirit, itemSO.spirit);

        if (itemSO.speed != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Speed, itemSO.speed);

        if (itemSO.dodge != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Dodge, itemSO.dodge);

        if (itemSO.luck != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Luck, itemSO.luck);

        if (itemSO.duration > 0)
            StartCoroutine(EffectTimer(itemSO, itemSO.duration));
    }

    private IEnumerator EffectTimer(ItemSO itemSO,float duration)
        {
            yield return new WaitForSeconds(duration);
        // 移除临时效果（反向加成）
        if (itemSO.speed != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Speed, -itemSO.speed);

        if (itemSO.damage != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Damage, -itemSO.damage);

        if (itemSO.defense != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Defense, -itemSO.defense);

        if (itemSO.magic != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Magic, -itemSO.magic);

        if (itemSO.magicDefense != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.MagicDefense, -itemSO.magicDefense);

        if (itemSO.dodge != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Dodge, -itemSO.dodge);

        if (itemSO.luck != 0)
            StatsManager.Instance.UpdateStat(StatsManager.StatType.Luck, -itemSO.luck);
    }
    
}
