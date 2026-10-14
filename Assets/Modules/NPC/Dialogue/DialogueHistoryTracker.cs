using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHistoryTracker : MonoBehaviour
{
  
    private readonly HashSet<ActorSO> spokenNPCs = new HashSet<ActorSO>();

 

    public void RecordNPC(ActorSO actorSO)
    {
        spokenNPCs.Add(actorSO);
        Debug.Log("just spoke to " + actorSO.actorName);
    }


    public bool HasSpokenWith(ActorSO actorSO)
    {
        return spokenNPCs.Contains(actorSO);
    }

    // ===== 新游戏时清空对话历史 =====
    public void Clear()
    {
        spokenNPCs.Clear();
    }
}
