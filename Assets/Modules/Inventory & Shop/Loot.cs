using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;

    public bool canBePickedUp = true;
    public int quantity;


    //创建委托
    public static event Action<ItemSO, int> OnItemLooted;


    private void OnValidate()
    {
        if (itemSO == null)
            return;
        UpdateAppearance();
    }
    private void OnEnable()
    {
        // 每次从池中取出时，重置动画到 Idle 状态
        canBePickedUp = true;
        anim.Play("Idle");
    }
    public void Instantiate(ItemSO itemSO,int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = quantity;
        canBePickedUp=true;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        sr.sprite = itemSO.icon;
        this.name = itemSO.name;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canBePickedUp == true)
        {
            anim.Play("LootPickup");
            OnItemLooted?.Invoke(itemSO, quantity);

        }
    }
  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            canBePickedUp = true;
    }

    public void OnPickupAnimationComplete()
    {
        LootPool.Instance.ReleaseLoot(gameObject);
    }
}
