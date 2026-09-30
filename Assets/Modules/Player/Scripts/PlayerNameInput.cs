using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

   

    public void SavePlayerName()
    {
        string playerName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("名字不能为空");
            return;
        }

        StatsManager.Instance.PlayerName = playerName;    
    }  
}