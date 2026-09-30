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
        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
        UpdateHealthUI(0);
    }
    private void OnEnable()
    {
        StatsManager.Instance.OnHealthChanged += UpdateHealthUI;       
    }

    private void OnDisable()
    {
        StatsManager.Instance.OnHealthChanged -= UpdateHealthUI;   
    }
    public void UpdateHealthUI(int amount)
    {
        
        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
        healthTextAnim.Play("TextUpdate");
        
        healthSlider.maxValue = StatsManager.Instance.MaxHealth;
        healthSlider.value = StatsManager.Instance.CurrentHealth;
       
    }
}
