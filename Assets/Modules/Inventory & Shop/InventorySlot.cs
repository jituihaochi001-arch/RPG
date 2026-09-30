using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,IPointerClickHandler
{   
    public ItemSO itemSO;
    public int quantity;

    public Image itemImage;
    public TMP_Text quantityText; 
 
    //左键点击使用物品，右键点击丢弃物品 && 若处于商店界面，可出售物品栏物品
    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity > 0)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (ShopManager.Instance.IsOnShop)
                {
                    ShopManager.Instance.HandleSell(itemSO);
                    quantity--;
                    UpdateUI();
                }
                else
                {
                    if (itemSO.currentHealth > 0 && StatsManager.Instance.CurrentHealth >= StatsManager.Instance.MaxHealth)
                        return;
                    InventoryManager.Instance.UseItem(this);
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                InventoryManager.Instance.DropItem(this);
            }
        }
    }
    //更新界面
    public void UpdateUI()
    {
        if(quantity<=0)
            itemSO = null;
        if (itemSO != null)
        {
            itemImage.sprite = itemSO.icon;
            itemImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }
}
