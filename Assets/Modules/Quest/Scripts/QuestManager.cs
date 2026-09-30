using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    //此脚本负责与任务相关的逻辑处理

    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    private List<QuestSO> completeQuests = new();

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        QuestEvents.IsQuestComplete += IsQuestComplete;
    }
    private void OnDisable()
    {
        QuestEvents.IsQuestComplete -= IsQuestComplete;
    }
    #region Quest Accept logic
    //接受任务
    public bool IsQuestAccepted(QuestSO questSO)
    {
        return questProgress.ContainsKey(questSO);
    }

    public List<QuestSO> GetActiveQuests()
    {
        return new List<QuestSO>(questProgress.Keys);
    }
    
    public void AcceptQuest(QuestSO questSO)
    {
        questProgress[questSO] = new Dictionary<QuestObjective, int>();

        foreach (var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO, objective);
        }
    }
    #endregion

    #region Quest Complete Logic
    //检查目标任务是否完成
    public bool IsQuestComplete(QuestSO questSO)
    {
        if(!questProgress.TryGetValue(questSO,out var progressDict))
            return false;
        foreach(var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO,objective);
        }

        foreach(var objective in questSO.objectives)
        {
            if (progressDict[objective] < objective.requiredAmount)
                return false;
        }

        return true;
    }
    public void CompleteQuest(QuestSO questSO)
    {
       
        questProgress.Remove(questSO);
        completeQuests.Add(questSO);

        //提交任务物品
        foreach(var objective in questSO.objectives)
        {
            if(objective.targetItem != null && objective.requiredAmount > 0)
            {
                InventoryManager.Instance.RemoveItem(objective.targetItem,objective.requiredAmount);
            }
        }

        //获取任务奖励
        foreach(var reward in questSO.rewards)
        {
            InventoryManager.Instance.AddItem(reward.itemSO, reward.quantity);
        }
    }
    public bool IsCompletedQuests(QuestSO questSO)
    {
        return completeQuests.Contains(questSO);
    }
#endregion

    //更新任务进程
    public void UpdateObjectiveProgress(QuestSO questSO,QuestObjective objective)
    {
        if (!questProgress.ContainsKey(questSO))
            return;

        var progressDictionary = questProgress[questSO];
        //初始值为0
        int newAmount = 0;

        if (objective.targetItem != null)
            newAmount = InventoryManager.Instance.GetItemQuantity(objective.targetItem);
        else if (objective.targetLocation != null && GameManager.Instance.LocationHistoryTracker.HasVisited(objective.targetLocation))
            newAmount = objective.requiredAmount;
        else if (objective.targetActor != null && GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(objective.targetActor))
            newAmount = objective.requiredAmount;

        progressDictionary[objective] = newAmount;
    }
    //获取任务进度文本
    public string GetProgressText(QuestSO questSO,QuestObjective objective)
    {
        int currentAmount  = GetCurrentAmount(questSO, objective);

        if (currentAmount >= objective.requiredAmount)
            return "已完成";
        else if (objective.targetItem != null)
            return $"{currentAmount}/{objective.requiredAmount}";
        else
            return "进行中";

    }
    //获取当前任务进度数字
    public int GetCurrentAmount(QuestSO questSO,QuestObjective objective)
    {
        if (questProgress.TryGetValue(questSO, out var progressDictionary))
            if (progressDictionary.TryGetValue(objective,out int amount))
                return amount;
        return 0;
    }

    // ===== 存档方法 =====
    public QuestSaveData GetSaveData()
    {
        QuestSaveData saveData = new QuestSaveData();

        foreach (var kvp in questProgress)
        {
            QuestSO quest = kvp.Key;
            var objectiveDict = kvp.Value;

            QuestProgressSaveData questSave = new QuestProgressSaveData();
            questSave.questName = quest.questName;

            for (int i = 0; i < quest.objectives.Count; i++)
            {
                var objective = quest.objectives[i];
                int progress = objectiveDict.TryGetValue(objective, out int value) ? value : 0;

                questSave.objectives.Add(new ObjectiveProgressSaveData
                {
                    objectiveIndex = i,
                    currentAmount = progress
                });
            }

            saveData.activeQuests.Add(questSave);
        }

        foreach (var quest in completeQuests)
        {
            saveData.completedQuestNames.Add(quest.questName);
        }

        return saveData;
    }

    public void LoadSaveData(QuestSaveData saveData)
    {
        questProgress.Clear();
        completeQuests.Clear();

        foreach (string questName in saveData.completedQuestNames)
        {
            QuestSO quest = FindQuestByName(questName);
            if (quest != null) completeQuests.Add(quest);
        }

        foreach (var questSave in saveData.activeQuests)
        {
            QuestSO quest = SODatabase.Instance.GetQuest(questSave.questName);
            if (quest == null) continue;

            var objectiveDict = new Dictionary<QuestObjective, int>();

            foreach (var objSave in questSave.objectives)
            {
                if (objSave.objectiveIndex >= 0 && objSave.objectiveIndex < quest.objectives.Count)
                {
                    objectiveDict[quest.objectives[objSave.objectiveIndex]] = objSave.currentAmount;
                }
            }

            questProgress[quest] = objectiveDict;
        }
    }

    private QuestSO FindQuestByName(string questName)
    {
        QuestSO[] allQuests = Resources.FindObjectsOfTypeAll<QuestSO>();
        foreach (var quest in allQuests)
        {
            if (quest.questName == questName)
                return quest;
        }
        return null;
    }
}
