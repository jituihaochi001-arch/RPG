using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// ===== 存档数据结构 =====
[System.Serializable]
public class InventorySaveData
{
    public List<ItemSlotSaveData> slots = new List<ItemSlotSaveData>();
    public int gold;
}

[System.Serializable]
public class ItemSlotSaveData
{
    public string itemSOname;
    public int quantity;
    public int slotIndex;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public InventorySlot[] itemSlots;
    public UseItem useItem;
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
   

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);


        // ===== 注册为持久化对象 =====
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPersistentObject(gameObject);
        }
    }

    private void Start()
    {
        foreach(var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }


    //订阅收取物品事件
    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }
    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }
    //收集物品放到物品栏
    public void AddItem(ItemSO itemSO,int quantity)
    {
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();  
            return;
        }
        if (itemSO.isEXP)
        {
            QuestEvents.TriggerExperienceGained(quantity);
            return;
        }
        //检查相同物品栏
        foreach (var slot in itemSlots)
        {
            //保证收集物品数量不超过物品栏上限
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                int availableSpace = itemSO.stackSize - slot.quantity;
                int amountToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                slot.UpdateUI();
                if (quantity <= 0)
                    return;
            }
        }
        //检查空物品栏
        foreach (var slot in itemSlots)
        {
            if (slot.itemSO == null)
            {
                int amountToAdd = Mathf.Min(itemSO.stackSize,quantity);
                slot.itemSO = itemSO;
                slot.quantity = quantity;
                slot.UpdateUI();
                return;
            }
        }
        if (quantity>0)
            DropLoot(itemSO, quantity);                   
    }
    //移除需要提交的物品
    public void RemoveItem(ItemSO itemSO, int quantity)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slot = itemSlots[i];
            if(slot.itemSO != itemSO)
                continue;
            if(slot.quantity > quantity)
            {
                slot.quantity -= quantity;
                slot.UpdateUI();
                quantity = 0;
            }
            else
            {
                quantity -= slot.quantity;
                slot.itemSO = null;
                slot.quantity = 0;
                slot.UpdateUI();
            }
        }
    }


    //丢弃物品  
    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if(slot.quantity <= 0)
            slot.itemSO = null;
        slot.UpdateUI();
    }
    private void DropLoot(ItemSO itemSO, int quantity)
    {
        Transform player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Instantiate(itemSO, quantity);
    }
    //使用物品方法
    public void UseItem(InventorySlot slot)
    {
        if(slot.itemSO != null && slot.quantity >=0)
        {
            useItem.ApplyItemEffects(slot.itemSO);
            slot.quantity--;
            if(slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }

    //拥有物品
    public bool HasItem(ItemSO itemSO)
    {
        foreach (var slot in itemSlots)
        { 
            if(slot.itemSO == itemSO && slot.quantity > 0)
                return true;
        }
        return false;
    }
    //检查拥有物品数量
    public int GetItemQuantity(ItemSO itemSO)
    {
        int total = 0;
        foreach(var slot in itemSlots)
        {
            if (slot.itemSO == itemSO)
                total += slot.quantity;
        }

        return total;
    }

    #region 保存数据
    // ===== 数据转换方法 =====
    public InventorySaveData GetSaveData()
    {
        InventorySaveData data = new InventorySaveData();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slot = itemSlots[i];
            if (slot.itemSO != null && slot.quantity > 0)
            {
                data.slots.Add(new ItemSlotSaveData
                {
                    itemSOname = slot.itemSO.name,
                    quantity = slot.quantity,
                    slotIndex = i
                }
                ); 
                Debug.Log($"存档物品：{slot.itemSO.name} x{slot.quantity}，槽位：{i}");
            }
        }
        data.gold = gold;
        Debug.Log($"存档金币：{gold}，物品数量：{data.slots.Count}");
        return data;
      
    }

    public void LoadSaveData(InventorySaveData data)
    {
        // 清空所有格子
        foreach (var slot in itemSlots)
        {
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }

        gold = data.gold;
        goldText.text = gold.ToString();
        Debug.Log($"加载金币：{gold}");

        foreach (var slotData in data.slots)
        {
            if (slotData.slotIndex >= 0 && slotData.slotIndex < itemSlots.Length)
            {
                ItemSO itemSO = SODatabase.Instance.GetItem(slotData.itemSOname);

                if (itemSO != null)
                {
                    var slot = itemSlots[slotData.slotIndex];
                    slot.itemSO = itemSO;
                    slot.quantity = slotData.quantity;
                    slot.UpdateUI();
                    Debug.Log($"加载物品：{itemSO.name} x{slotData.quantity}个，槽位：{slotData.slotIndex}");
                }
                else
                {
                    Debug.LogWarning($"未找到物品: {slotData.itemSOname}");
                }
            }
        }
    }
    #endregion
}


