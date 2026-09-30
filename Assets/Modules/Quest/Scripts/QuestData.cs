using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===== 任务存档数据 =====
[System.Serializable]
public class QuestSaveData
{
    public List<QuestProgressSaveData> activeQuests = new List<QuestProgressSaveData>();
    public List<string> completedQuestNames = new List<string>();
}

[System.Serializable]
public class QuestProgressSaveData
{
    public string questName;
    public List<ObjectiveProgressSaveData> objectives = new List<ObjectiveProgressSaveData>();
}

[System.Serializable]
public class ObjectiveProgressSaveData
{
    public int objectiveIndex;
    public int currentAmount;
}