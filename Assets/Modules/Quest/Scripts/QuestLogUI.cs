using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    //此脚本负责更新界面和管理界面输入

    [SerializeField] private QuestManager questManager;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots;
    [SerializeField] private QuestRewardSlot[] rewardSlots;

    private QuestSO questSO;
    [SerializeField] private QuestSO noAvailiableQuestSO;
    [SerializeField] private QuestLogSlot[] questSlots;

    [SerializeField] private CanvasGroup questCanvasGroup;
    [SerializeField] private CanvasGroup questDetailsCanvasGroup;
    [SerializeField] private CanvasGroup acceptCanvasGroup;
    [SerializeField] private CanvasGroup declineCanvasGroup;
    [SerializeField] private CanvasGroup completeCanvasGroup;

    public Animator anim;
    private void OnEnable()
    {
        QuestEvents.OnQuestOfferRequested += ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested += ShowQuestTurnIn;
    }
    private void OnDisable()
    {
        QuestEvents.OnQuestOfferRequested -= ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested -= ShowQuestTurnIn;
    }

    #region 展示任务画布

    //展示任务详情画布
    public void ShowQuestOffer(QuestSO incomingQuestSO)
    {
        anim.Play("Quest_appear");
        if (questManager.IsQuestAccepted(incomingQuestSO) || questManager.IsCompletedQuests(incomingQuestSO)) 
        { 
            questSO = noAvailiableQuestSO;
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }
        else
        {
            questSO = incomingQuestSO;
            SetCanvasState(acceptCanvasGroup, true);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }
        HandleQuestClicked(questSO);

        SetCanvasState(questCanvasGroup,true);
              
    }
    //展示任务完成画布
    public void ShowQuestTurnIn(QuestSO incomingQuestSO)
    {
        questSO = incomingQuestSO;

        HandleQuestClicked(questSO);

        SetCanvasState(completeCanvasGroup, true);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);
        SetCanvasState(questCanvasGroup, true);
    }

    #endregion


    #region 点击按钮方法
    public void OnAcceptClicked()
    {
        QuestEvents.OnQuestAccepted?.Invoke(questSO);

        questManager.AcceptQuest(questSO);
        SetCanvasState(completeCanvasGroup, false);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);

        RefreshQuestList();
        HandleQuestClicked(questSO);
    }

    public void OnCloseClicked()
    {
        SetCanvasState(questCanvasGroup, false);
    }

    public void OnCompleteQuestClicked()
    {
       
        questManager.CompleteQuest(questSO);    
        RefreshQuestList();
        SetCanvasState(questDetailsCanvasGroup,false);
        SetCanvasState(completeCanvasGroup,false);
    }

    #endregion
    private void SetCanvasState(CanvasGroup group,bool activate)
    {
        group.alpha = activate ? 1 : 0;
        group.blocksRaycasts = activate;
        group.interactable = activate;

    }
    //刷新任务列表
    public void RefreshQuestList()
    {
        List<QuestSO> activeQuests = questManager.GetActiveQuests();

        for (int i = 0; i < questSlots.Length; i++)
        { 
            if(i < activeQuests.Count)
            {
                questSlots[i].SetQuest(activeQuests[i]);
            }
            else
            {
                questSlots[i].ClearSlot();
            }
        }
    }

    //点击任务列表里的任务时
    public void HandleQuestClicked(QuestSO  questSO)
    {
        this.questSO = questSO;

        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDescription;

        SetCanvasState(questDetailsCanvasGroup, true);
        anim.Play("Quest_appear");

        DisplayObjective();
        DisplayReward();        
    }

    private void DisplayObjective()
    {
        for(int i = 0; i < objectiveSlots.Length; i++)
        {
            if(i < questSO.objectives.Count)
            {
                var objective = questSO.objectives[i];
                questManager.UpdateObjectiveProgress(questSO, objective);

                int currentAmount = questManager.GetCurrentAmount(questSO, objective);
                string progress = questManager.GetProgressText(questSO, objective);
                bool isComplete = currentAmount >= objective.requiredAmount;

                objectiveSlots[i].gameObject.SetActive(true);
                objectiveSlots[i].RefreshObjectives(objective.description,progress,isComplete);
            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void DisplayReward()
    {
        for (int i = 0;i < rewardSlots.Length; i++)
        {
            if (i < questSO.rewards.Count)
            {
                var reward = questSO.rewards[i];
                rewardSlots[i].DisplayReward(reward.itemSO.icon,reward.quantity);
                rewardSlots[i].gameObject.SetActive(true);
            }
            else
            {
                rewardSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
