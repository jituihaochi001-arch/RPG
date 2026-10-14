using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class QuestEvents 
{
    public static Action<QuestSO> OnQuestOfferRequested;
    public static Action<QuestSO> OnQuestTurnInRequested;
    public static Action<QuestSO> OnQuestAccepted;


    public static Func<QuestSO, bool> IsQuestComplete;

    //获取经验值事件
    public static event Action<int> OnExperienceGained;
    public static void TriggerExperienceGained(int amount)
    {
        OnExperienceGained?.Invoke(amount);
    }

    //任务数据重置事件（新游戏时用于刷新任务日志 UI）
    public static event Action OnQuestDataReset;
    public static void TriggerQuestDataReset()
    {
        OnQuestDataReset?.Invoke();
    }
}
