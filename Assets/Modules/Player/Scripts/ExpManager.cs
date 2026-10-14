using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class ExpManager : MonoBehaviour
{
    public static ExpManager Instance;
    public int level;
    public int currentExp;
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.5f;
    public Slider expSlider;
    public TMP_Text currentLevelText;
    public static event Action<int> OnLevelUp;

    // 记录升级曲线的初始经验需求，避免新游戏重置时使用了已经成长过的数值
    private int defaultExpToLevel;

    private void Awake()
    {
        Instance = this;
        defaultExpToLevel = expToLevel;
    }
    private void Start()
    {
        UpdateUI();
    }
   
    private void OnEnable()
    {
        EnemyHealth.OnMonsterDefeated += GainExperience;
        QuestEvents.OnExperienceGained += GainExperience;
    }
    private void OnDisable()
    {
        EnemyHealth.OnMonsterDefeated -= GainExperience;
        QuestEvents.OnExperienceGained -= GainExperience;
    }
    public void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp > expToLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }
    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);
        OnLevelUp?.Invoke(1);
    }

    // ===== 新游戏时重置等级与经验 =====
    public void ResetToDefault()
    {
        level = 0;
        currentExp = 0;
        expToLevel = defaultExpToLevel;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expToLevel;
            expSlider.value = currentExp;
        }

        if (currentLevelText != null)
        {
            currentLevelText.text = "Level: "+level;
        }
    }
}
