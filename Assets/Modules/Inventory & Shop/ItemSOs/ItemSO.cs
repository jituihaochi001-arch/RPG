using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;

    public bool isGold;
    public bool isEXP;
    public bool isFood;
    public int stackSize = 64;

    [Header("属性加成")]
    public int currentHealth;      // 恢复/扣除血量
    public int maxHealth;          // 增加/减少最大生命值
    public int damage;             // 增加/减少攻击力
    public int magic;              // 增加/减少魔力
    public int defense;            // 增加/减少防御
    public int magicDefense;       // 增加/减少魔防
    public int spirit;             // 增加/减少精神
    public int speed;              // 增加/减少速度
    public int vitality;           // 增加/减少体质
    public int dodge;              // 增加/减少闪避
    public int luck;               // 增加/减少幸运

    [Header("持续时间")]
    public float duration;
}
