using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShopManager : MonoBehaviour
{  
    public static ShopManager Instance;

    [SerializeField] private ShopSlot[] shopSlots;

    //标志位，检查是否处于商店界面
    public bool IsOnShop = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        // ===== 注册为持久化对象 =====
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPersistentObject(gameObject);
        }
    }
    private void OnEnable()
    {
        ShopEvents.OnItemBought += HandleBuy;
        ShopEvents.OnItemSold += HandleSell;
    }

    private void OnDisable()
    {
        ShopEvents.OnItemBought -= HandleBuy;
        ShopEvents.OnItemSold -= HandleSell;
    }
    // ===== 填充商店物品 =====
    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }
        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    //购买商品
    public void HandleBuy(ItemSO itemSO, int price)
    {
        if (itemSO != null && InventoryManager.Instance.gold >= price)
        {
            if (HasSpaceForItem(itemSO))
            {
                InventoryManager.Instance.gold -= price;
                InventoryManager.Instance.goldText.text = InventoryManager.Instance.gold.ToString();
                InventoryManager.Instance.AddItem(itemSO,1);
                Debug.Log($"购买了1个 {itemSO.itemName}");
            }
        }
    }
    //检查是否有物品栏空位来存放商品
    private bool HasSpaceForItem(ItemSO itemSO) 
    {
        foreach (var slot in InventoryManager.Instance.itemSlots) 
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                return true; 
            else if (slot.itemSO == null)
                return true;
        }
        return false;
    } 
    //出售物品
    public void HandleSell(ItemSO itemSO)
    {
        if (itemSO == null)
            return;
        foreach(var slot in shopSlots)
        {
            if(slot.itemSO == itemSO)
            {
                // 计算出售价格（原价的 70%）
                int sellPrice = Mathf.RoundToInt(slot.price * 0.7f);

                // 如果出售价格低于 1，按 1 算
                if (sellPrice < 1) sellPrice = 1;

                InventoryManager.Instance.gold += sellPrice;
                InventoryManager.Instance.goldText.text = InventoryManager.Instance.gold.ToString();
                Debug.Log($"出售了1个 {itemSO.itemName}");
                return;
            }
        }
    }

}
[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}