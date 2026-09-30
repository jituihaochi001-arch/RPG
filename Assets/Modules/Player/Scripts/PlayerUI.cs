using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text nameText;
    private void Start()
    {
        // ≥ı ºªØœ‘ æ
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.OnNameChanged += UpdateNameText;
        }
    }


    private void OnDisable()
    {     
           StatsManager.Instance.OnNameChanged -= UpdateNameText;      
    }
    public void UpdateNameText(string playerName)
    {
        nameText.text = playerName;
    }
    
  
   
}
