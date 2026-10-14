using UnityEngine;

/// <summary>
/// 新游戏初始化入口。
/// 所有会跨场景常驻的运行时数据都从这里统一重置，避免上一个存档的数值被带入新游戏。
/// </summary>
public static class NewGameInitializer
{
    // ===== 重置所有新游戏需要初始化的数据 =====
    public static void ResetAll()
    {
        // 清理场景跳转参数与 NPC/地点历史记录
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetNewGameState();
        }

        // 取消尚未应用的读档数据，避免新游戏被上一次读档流程覆盖
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CancelPendingLoad();
        }

        // 重置角色属性、等级经验、技能树、背包、任务与商店状态
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetToDefault();
        }

        if (ExpManager.Instance != null)
        {
            ExpManager.Instance.ResetToDefault();
        }

        if (SkillTreeManager.Instance != null)
        {
            SkillTreeManager.Instance.ResetToDefault();
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetToDefault();
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ResetToDefault();
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ResetToDefault();
        }

        // 注意：音量等 PlayerPrefs 设置属于玩家偏好，不在新游戏重置范围内。
    }
}
