using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private TMP_Text slotText;

    // 存储该槽位的存档时间（从存档文件中读取）
    private string saveTimestamp;

    private void Start()
    {
        UpdateUI();
    }
    public void OnSlotClicked()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (UIManager.Instance.isSave == false)
        {
            // ===== 主菜单场景：载入存档 =====
            OnLoadButtonClick();
        }
        else
        {
            // ===== 游戏场景：保存存档 =====
            OnSaveButtonClick();
        }
    }
    public void OnSaveButtonClick()
    {
        SaveManager.Instance.SaveGame(slotIndex);
        UpdateUI();
    }
    public void OnLoadButtonClick()
    {
        bool hasSave = SaveManager.Instance.HasSave(slotIndex);

        if (hasSave)
        {
            // 有存档 → 直接载入
            SaveManager.Instance.LoadGame(slotIndex);
        }
        else
        {
            return;
            
        }
    }
    public void OnDeleteButtonClick()
    {
        SaveManager.Instance.DeleteSave(slotIndex);
        UpdateUI();
    }

    // ===== 新建存档 =====
    private void CreateNewSave()
    {
        // 获取当前时间
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveTimestamp = currentTime;

        // 执行存档
        SaveManager.Instance.SaveGame(slotIndex);

        // 更新 UI
        UpdateUI();

        Debug.Log($"已在存档位 {slotIndex + 1} 新建存档，时间: {currentTime}");
    }
    private void UpdateUI()
    {
        bool hasSave = SaveManager.Instance.HasSave(slotIndex);
        if (hasSave)
        {
            // 读取存档时间（从 SaveManager 获取）
            saveTimestamp = SaveManager.Instance.GetSaveTimestamp(slotIndex);
            if (string.IsNullOrEmpty(saveTimestamp))
            {
                saveTimestamp = "未知时间";
            }
            slotText.text = $"存档位 {slotIndex + 1}\n<size=40>{saveTimestamp}</size>";
        }
        else
        {
            slotText.text = $"存档位 {slotIndex + 1}\n<size=40>空</size>";
        }
    }
}