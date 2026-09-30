using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
   
    public ItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text PriceText;
    public Image itemImage;

    [SerializeField] private ShopInfo shopInfo;

    public int price;
    //初始化商店
    public void Initialize(ItemSO newitemSO,int price)
    {
        //填充商品信息
        itemSO = newitemSO;
        itemImage.sprite = itemSO.icon;
        itemNameText.text = itemSO.itemName;
        this.price = price;
        PriceText.text = price.ToString();
    }
    //点击购买
    public void OnBuyButtonClicked()
    {
        if (itemSO != null)
        {
            ShopEvents.TriggerItemBought(itemSO, price);
        }
    }
    public void OnSellButtonClicked()
    {
        if (itemSO != null)
        {
            ShopEvents.TriggerItemSold(itemSO);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemSO != null) 
            shopInfo.ShowItemInfo(itemSO);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            shopInfo.HideItemInfo(itemSO);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemSO != null)
            shopInfo.FolowMouse();
    }
}
