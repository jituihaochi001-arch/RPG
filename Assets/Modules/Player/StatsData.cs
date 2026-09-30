using System;

[System.Serializable]
public class StatsData
{
    // ===== 基础属性 =====
    public string playerName = "冒险者";

    // ===== 战斗属性 =====
    public int health = 20;
    public int maxHealth = 20;
    public int damage = 5;         // 物理攻击（力量）
    public int magic = 5;          // 魔法攻击（魔力）
    public int defense = 2;        // 物理防御
    public int magicDefense = 2;   // 魔法防御

    // ===== 辅助属性 =====
    public int spirit = 1;         // 精神（影响MP）
    public int speed = 5;          // 速度
    public int vitality = 1;       // 体质（影响HP）
    public int dodge = 1;          // 闪避
    public int luck = 1;           // 幸运

    // ===== 战斗配置 =====
    public float weaponRange = 2f;
    public float knockbackForce = 5f;
    public float knockbackTime = 0.5f;
    public float stunTime = 0.3f;
}