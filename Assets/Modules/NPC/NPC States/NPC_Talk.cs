using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;

    public List<DialogueSO> conversations;
    public DialogueSO currentConversation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        QuestEvents.OnQuestAccepted += OnQuestAccepted_RemoveOfferings;
    }
    private void OnDestroy()
    {
        QuestEvents.OnQuestAccepted -= OnQuestAccepted_RemoveOfferings;
    }
    private void OnEnable()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.Play("Idle");
        interactAnim.Play("Open");
    }
    private void OnDisable()
    {
        rb.isKinematic = false;
        interactAnim.Play("Close");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (DialogueManager.instance.isDialogueActives)
            {
                DialogueManager.instance.AdvanceDialogue();
            }

            else
            {
                //检查是否已过对话冷却时间
                if (DialogueManager.instance.CanStartDialogue())
                {
                    CheckForNewConversation();
                    DialogueManager.instance.StartDialogue(currentConversation);
                }
            }

        }
    }
    //检查新对话
    private void CheckForNewConversation()
    {
        for (int i = 0; i < conversations.Count; i++)
        {
            var convo = conversations[i];
            if (convo != null && convo.IsConditionMet())
            {
                currentConversation = convo;

                //检查是否需要完成一次对话后移除
                if(convo.removeAfterPlay)
                    conversations.RemoveAt(i); 
                //检查是否相关其他对话需要移除
                if(convo.removeTheseOnPlay != null && convo.removeTheseOnPlay.Count >0)
                {
                    foreach(var dialogueSO  in convo.removeTheseOnPlay)
                    {
                        conversations.Remove(dialogueSO);
                    }
                }    
                break;
            }
        }

    }
    private void OnQuestAccepted_RemoveOfferings(QuestSO acceptedQuest)
    {
        for (int i = conversations.Count - 1; i >= 0; i--) 
        {
            var convo = conversations[i];
            if(convo == null) continue;
            if (convo.offerQuestOnEnd == acceptedQuest)
                conversations.RemoveAt(i);
        }
    }

}
