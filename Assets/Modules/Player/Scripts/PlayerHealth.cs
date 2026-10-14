using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Animator healthTextAnim;
    public TMP_Text healthText;
    public Slider healthSlider;

    private void Start()
    {
        // 统一走 UpdateHealthUI，避免重复维护刷新逻辑
        UpdateHealthUI(0);
    }
    private void OnEnable()
    {
        if (StatsManager.Instance == null) return;

        StatsManager.Instance.OnHealthChanged += UpdateHealthUI;
        // 面板/对象重新启用时立即刷新一次，避免禁用期间错过属性变化
        UpdateHealthUI(0);
    }

    private void OnDisable()
    {
        if (StatsManager.Instance == null) return;

        StatsManager.Instance.OnHealthChanged -= UpdateHealthUI;
    }
    public void UpdateHealthUI(int amount)
    {
        if (StatsManager.Instance == null) return;

        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
        if (healthTextAnim != null)
        {
            healthTextAnim.Play("TextUpdate");
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = StatsManager.Instance.MaxHealth;
            healthSlider.value = StatsManager.Instance.CurrentHealth;
        }
       
    }
}
