using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header("UI Reference")]
    public CanvasGroup canvasGroup;
    public Image potrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public Button[] choiceButtons;

    public bool isDialogueActives = false;

    private DialogueSO currentDialogue;
    private int dialogueIndex;

    private float lastDialogueEndTime;
    private float diagueCooldown = .1f;

    public static DialogueManager instance;
    private void Awake()
    {        
        instance = this;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        foreach (var button in choiceButtons)
            button.gameObject.SetActive(false);
    }
    //设置对话冷却时间，防止重新触发对话
    public bool CanStartDialogue()
    {
        return Time.unscaledTime - lastDialogueEndTime >= diagueCooldown;           
    }
    //开始新对话
    public void StartDialogue(DialogueSO dialogueSO)
    {     
        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActives = true;
        ShowDialogue();
    }
    //推进对话
    public void AdvanceDialogue()
    {
        if (dialogueIndex < currentDialogue.lines.Length)
            ShowDialogue();
        else
            ShowChoices();

    }
    //展示对话
    private void ShowDialogue()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        DialogueLine line = currentDialogue.lines[dialogueIndex];

        //记录对话的NPC
        GameManager.Instance.DialogueHistoryTracker.RecordNPC(line.speaker);

        // 判空保护
        if (line.speaker != null)
        {
            potrait.sprite = line.speaker.potrait;
            actorName.text = line.speaker.actorName;
        }
        else
        {
            potrait.sprite = null;
            actorName.text = "???";
        }

        dialogueText.text = line.text;        

        dialogueIndex++;
    }

    private void ShowChoices()
    {
        ClearChoices();

        if (currentDialogue.options.Length > 0)
        {
            for (int i = 0; i < currentDialogue.options.Length; i++)
            {
                int index = i;
                DialogueOption option = currentDialogue.options[index];  // 每次循环创建一个新的局部变量，防止闭包陷阱
                //设置文本
                choiceButtons[index].GetComponentInChildren<TMP_Text>().text = option.optionText;
                choiceButtons[index].gameObject.SetActive(true);
                //绑定事件
                choiceButtons[index].onClick.RemoveAllListeners();
                choiceButtons[index].onClick.AddListener(() => ChooseOption(option.nextDialogue));
               
            }
            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
        }
        else
        {
            if (currentDialogue.offerQuestOnEnd != null) 
            {
                EndDialogue();
                QuestEvents.OnQuestOfferRequested(currentDialogue.offerQuestOnEnd);
            }
            else
            {
                choiceButtons[0].GetComponentInChildren<TMP_Text>().text = "End";
                choiceButtons[0].onClick.AddListener(EndDialogue);
                choiceButtons[0].gameObject.SetActive(true);
                //如果结束选项存在，持续选中
                EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
            }
        }                  
    }

    private void ChooseOption(DialogueSO dialogueSO)
    {
        if (dialogueSO != null)
        {
            ClearChoices();
            StartDialogue(dialogueSO);
        }
        else
        {
            EndDialogue();
        }
    }

    //结束对话
    private void EndDialogue()
    {
        dialogueIndex = 0;
        isDialogueActives = false;
        ClearChoices();

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        lastDialogueEndTime = Time.unscaledTime;
    }

    private void ClearChoices()
    {
        foreach (var button in choiceButtons) 
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }
   
}
