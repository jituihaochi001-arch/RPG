using System;
using System.Collections.Generic;

public static class ShopEvents
{
    // 商店打开/关闭
    public static event Action<ShopData> OnShopOpened;
    public static event Action OnShopClosed;

    // ===== 新增：标签页切换事件 =====
    public static event Action<ShopTab> OnShopTabChanged;

    // 交易事件
    public static event Action<ItemSO, int> OnItemBought;  // item, price, quantity
    public static event Action<ItemSO> OnItemSold;    // item, price, quantity

    public static void TriggerShopOpened(ShopData data)
    {
        OnShopOpened?.Invoke(data);
    }
    public static void TriggerShopClosed()
    {
        OnShopClosed?.Invoke();
    }
    public static void TriggerShopTabChanged(ShopTab tab)
    {
        OnShopTabChanged?.Invoke(tab);
    }
    public static void TriggerItemBought(ItemSO item, int price)
    {
        OnItemBought?.Invoke(item, price);
    }

    public static void TriggerItemSold(ItemSO item)
    {
        OnItemSold?.Invoke(item);
    }
}

// ===== 商店数据 =====
[System.Serializable]
public class ShopData
{
    public List<ShopItems> items = new List<ShopItems>();
    public List<ShopItems> weapons = new List<ShopItems>();
    public List<ShopItems> armours = new List<ShopItems>();
}