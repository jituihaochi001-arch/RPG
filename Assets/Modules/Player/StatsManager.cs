using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("UI References")]
    [SerializeField] private StatsUI statsUI;
    [SerializeField] private TMP_Text healthText;

    // ===== 运行时数据 =====
    private StatsData data = new StatsData();

    // ===== 属性访问器 =====
    public int CurrentHealth => data.health;
    public int MaxHealth => data.maxHealth;
    public int Damage => data.damage;
    public int Magic => data.magic;
    public int Defense => data.defense;
    public int MagicDefense => data.magicDefense;
    public int Spirit => data.spirit;
    public int Speed => data.speed;
    public int Vitality => data.vitality;
    public int Dodge => data.dodge;
    public int Luck => data.luck;
    public float WeaponRange => data.weaponRange;
    public float KnockbackForce => data.knockbackForce;
    public float KnockbackTime => data.knockbackTime;
    public float StunTime => data.stunTime;

    // 事件定义（玩家血量变化，玩家死亡）
    public event System.Action<int> OnHealthChanged;
    public event System.Action OnPlayerDied;
    // 定义事件：当名字改变时触发
    public event System.Action<string> OnNameChanged;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else 
        { Destroy(gameObject); }


        // ===== 注册为持久化对象 =====
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPersistentObject(gameObject);
        }
    }
    public void UpdateStat(StatType type, int amount)
    {
        switch (type)
        {
            case StatType.Health:
                data.health = Mathf.Clamp(data.health + amount, 0, data.maxHealth);
                break;
            case StatType.MaxHealth:
                data.maxHealth += amount;
                OnHealthChanged?.Invoke(0);
                break;
            case StatType.Damage:
                data.damage += amount;
                break;
            case StatType.Magic:
                data.magic += amount;
                break;
            case StatType.Defense:
                data.defense += amount;
                break;
            case StatType.MagicDefense:
                data.magicDefense += amount;
                break;
            case StatType.Spirit:
                data.spirit += amount;
                break;
            case StatType.Speed:
                data.speed += amount;
                break;
            case StatType.Vitality:
                data.vitality += amount;
                break;
            case StatType.Dodge:
                data.dodge += amount;
                break;
            case StatType.Luck:
                data.luck += amount;
                break;
            default:
                return;
        }

        UpdateUI();
    }
    //当前血量变化
    public void ChangeHealth(int amount)
    {
        data.health = Mathf.Clamp(data.health + amount, 0, data.maxHealth);
        OnHealthChanged?.Invoke(data.health);
        UpdateUI();

        if (data.health <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }
    //名字改变时触发
    public string PlayerName
    {
        get => data.playerName;
        set
        {
            data.playerName = value;
            OnNameChanged?.Invoke(data.playerName);  // 通知所有订阅者
        }
    }
    // ===== 更新 UI =====
    private void UpdateUI()
    {       
        if (statsUI != null)
        {
            statsUI.UpdateAllStats();
        }
    }

    // ===== 获取存档数据 =====
    public StatsData GetSaveData()
    {
        return new StatsData
        {
            playerName = data.playerName,
            health = data.health,
            maxHealth = data.maxHealth,
            damage = data.damage,
            magic = data.magic,
            defense = data.defense,
            magicDefense = data.magicDefense,
            spirit = data.spirit,
            speed = data.speed,
            vitality = data.vitality,
            dodge = data.dodge,
            luck = data.luck,
            weaponRange = data.weaponRange,
            knockbackForce = data.knockbackForce,
            knockbackTime = data.knockbackTime,
            stunTime = data.stunTime
        };
    }

    // ===== 加载存档数据 =====
    public void LoadSaveData(StatsData loadedData)
    {
        data = loadedData;
        UpdateUI();
        OnHealthChanged?.Invoke(data.health);
    }
    public enum StatType
    {
        Health,
        MaxHealth,
        Damage,
        Magic,
        Defense,
        MagicDefense,
        Spirit,
        Speed,
        Vitality,
        Dodge,
        Luck
    }
}
