using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup startgroup;
    [SerializeField] CanvasGroup settinggroup;
    
    private void OnEnable()
    {
        SetPanelState(settinggroup, true);

        SetPanelState(startgroup, false);
    }  
    public void OnClickedCloseButton()
    {
        
            SetPanelState(settinggroup, false);
            SetPanelState(startgroup, true);
        
    }
    private void SetPanelState(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }
}
