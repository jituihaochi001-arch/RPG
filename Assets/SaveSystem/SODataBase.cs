using System.Collections.Generic;
using UnityEngine;

public class SODatabase : MonoBehaviour
{
    public static SODatabase Instance;

    // ===== 所有 SO 数据 =====
    [Header("Items")]
    [SerializeField] private List<ItemSO> allItems = new List<ItemSO>();

    [Header("Quests")]
    [SerializeField] private List<QuestSO> allQuests = new List<QuestSO>();

    [Header("Dialogues")]
    [SerializeField] private List<DialogueSO> allDialogues = new List<DialogueSO>();

    [Header("Actors")]
    [SerializeField] private List<ActorSO> allActors = new List<ActorSO>();

    // ===== 字典缓存 =====
    private Dictionary<string, ItemSO> itemCache = new Dictionary<string, ItemSO>();
    private Dictionary<string, QuestSO> questCache = new Dictionary<string, QuestSO>();
    private Dictionary<string, DialogueSO> dialogueCache = new Dictionary<string, DialogueSO>();
    private Dictionary<string, ActorSO> actorCache = new Dictionary<string, ActorSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildAllCaches();
    }

    private void BuildAllCaches()
    {
        BuildItemCache();
        BuildQuestCache();
        BuildDialogueCache();
        BuildActorCache();
    }

    // ===== 各类型缓存 =====
    private void BuildItemCache()
    {
        itemCache.Clear();
        foreach (var item in allItems)
        {
            if (item != null && !itemCache.ContainsKey(item.name))
                itemCache[item.name] = item;
        }
    }

    private void BuildQuestCache()
    {
        questCache.Clear();
        foreach (var quest in allQuests)
        {
            if (quest != null && !questCache.ContainsKey(quest.questName))
                questCache[quest.questName] = quest;
        }
    }

    private void BuildDialogueCache()
    {
        dialogueCache.Clear();
        foreach (var dialogue in allDialogues)
        {
            if (dialogue != null && !dialogueCache.ContainsKey(dialogue.name))
                dialogueCache[dialogue.name] = dialogue;
        }
    }

    private void BuildActorCache()
    {
        actorCache.Clear();
        foreach (var actor in allActors)
        {
            if (actor != null && !actorCache.ContainsKey(actor.name))
                actorCache[actor.name] = actor;
        }
    }

    // ===== 公开查找方法 =====
    public ItemSO GetItem(string name)
    {
        itemCache.TryGetValue(name, out ItemSO itemSO);
        return itemSO;
    }

    public QuestSO GetQuest(string name)
    {
        questCache.TryGetValue(name, out QuestSO result);
        return result;
    }

    public DialogueSO GetDialogue(string name)
    {
        dialogueCache.TryGetValue(name, out DialogueSO result);
        return result;
    }

    public ActorSO GetActor(string name)
    {
        actorCache.TryGetValue(name, out ActorSO result);
        return result;
    }
}