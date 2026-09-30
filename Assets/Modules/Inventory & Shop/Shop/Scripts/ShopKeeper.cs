using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] private ShopData shopData;

    [SerializeField] private Camera shopkeeperCam;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0,0,-1);

    public static ShopKeeper currentShopKeeper;
    public Animator anim;

    private bool playerInRange;
    private bool isShopOpen;

    private void Update()
    {   //进入商人范围内按F进入商店界面
        if (playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {   //打开商店界面
                if (!isShopOpen)
                {
                    OpenShop();
                    shopkeeperCam.transform.position = this.transform.position + cameraOffset;                 
                    shopkeeperCam.gameObject.SetActive(true);                   
                }
            }
            //关闭商店界面
            else if (Input.GetButtonDown("Cancel"))
            {
                CloseShop();
                shopkeeperCam.gameObject.SetActive(false);              
            }
            
        }
    }
    private void OpenShop()
    {
        isShopOpen = true;
        Time.timeScale = 0;
        currentShopKeeper = this;
        ShopManager.Instance.IsOnShop = true;
        ShopEvents.TriggerShopOpened(shopData);
    }
    private void CloseShop()
    {
        isShopOpen = false;
        Time.timeScale = 1;
        currentShopKeeper = null;
        ShopManager.Instance.IsOnShop = false;
        ShopEvents.TriggerShopClosed();
    }
    // ===== 供 ShopButtonToggles 调用 =====
    public void OpenItemShop()
    {
        ShopEvents.TriggerShopTabChanged(ShopTab.Items);
    }

    public void OpenWeaponShop()
    {
        ShopEvents.TriggerShopTabChanged(ShopTab.Weapons);
    }

    public void OpenArmourShop()
    {
        ShopEvents.TriggerShopTabChanged(ShopTab.Armours);
    }

    //进入范围触发
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
            playerInRange = true;     
        }
    }
    //离开范围触发
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
public enum ShopTab
{
    Items,
    Weapons,
    Armours
}
