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



}
