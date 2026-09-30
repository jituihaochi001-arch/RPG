using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup shopCanvasGroup;

    private ShopData currentShopData;
    private ShopTab currentTab;

    private void OnEnable()
    {
        ShopEvents.OnShopOpened += OnShopOpened;
        ShopEvents.OnShopClosed += OnShopClosed;
        ShopEvents.OnShopTabChanged += OnShopTabChanged;
    }

    private void OnDisable()
    {
        ShopEvents.OnShopOpened -= OnShopOpened;
        ShopEvents.OnShopClosed -= OnShopClosed;
        ShopEvents.OnShopTabChanged -= OnShopTabChanged;
    }
    //打开商店
    private void OnShopOpened(ShopData data)
    {
        currentShopData = data;
        ShowShop(true);
        OnShopTabChanged(ShopTab.Items);
    }
    //关闭商店
    private void OnShopClosed()
    {
        ShowShop(false);
    }

    private void OnShopTabChanged(ShopTab tab)
    {
        currentTab = tab;
        List<ShopItems> items = GetTabItems(tab);
        ShopManager.Instance.PopulateShopItems(items);
    }

    private void ShowShop(bool show)
    {
        shopCanvasGroup.alpha = show ? 1 : 0;
        shopCanvasGroup.blocksRaycasts = show;
        shopCanvasGroup.interactable = show;
    }
   
    private List<ShopItems> GetTabItems(ShopTab tab)
    {
        switch (tab)
        {
            case ShopTab.Items: return currentShopData.items;
            case ShopTab.Weapons: return currentShopData.weapons;
            case ShopTab.Armours: return currentShopData.armours;
            default: return null;
        }
    }
}