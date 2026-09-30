using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCanvas : MonoBehaviour
{
   
    public void Close()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Start Scene")
        {
            // ===== 主菜单场景=====
            closeCanvas();
            StartManager.instance.StartTheDefaultCanvas();
        }
        else
        {
            // ===== 游戏场景=====
            UIManager.Instance.ClosePanel();
        }
    }

    public void closeCanvas()
    {
        CanvasGroup canvas = GetComponentInParent<CanvasGroup>();       
        SetPanelState(canvas, false);
        Time.timeScale = 1;
    }

    private void SetPanelState(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }
}
