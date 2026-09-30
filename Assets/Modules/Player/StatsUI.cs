using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private CanvasGroup statsCanvas;
    [SerializeField] private TMP_Text[] statTexts;  

    private bool statsOpen = false;

    private void Start()
    {
        UpdateAllStats();
    }
    private void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
        {   if (statsOpen)
            {
                Time.timeScale = 1;
                UpdateAllStats();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
                statsOpen = false;
            }
            else
            {
                Time.timeScale = 0;
                UpdateAllStats();
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                statsOpen =true;
            }
        }
    }

    public void UpdateAllStats()
    {
        var stats = StatsManager.Instance;

        statTexts[0].text = $"力量: {stats.Damage}";
        statTexts[1].text = $"魔力: {stats.Magic}";
        statTexts[2].text = $"防御: {stats.Defense}";
        statTexts[3].text = $"魔防: {stats.MagicDefense}";
        statTexts[4].text = $"生命: {stats.MaxHealth}";
        statTexts[5].text = $"精神: {stats.Spirit}";
        statTexts[6].text = $"速度: {stats.Speed}";
        statTexts[7].text = $"体质: {stats.Vitality}";
        statTexts[8].text = $"闪避: {stats.Dodge}";
        statTexts[9].text = $"幸运: {stats.Luck}";
    }
}
