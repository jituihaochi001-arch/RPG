using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] CanvasGroup startGroup;
    [SerializeField] CanvasGroup newGroup;
    [SerializeField] CanvasGroup settingGroup;
    [SerializeField] CanvasGroup exitGroup;
    [SerializeField] CanvasGroup savecanvas;

    public static StartManager instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        savecanvas = GameObject.Find("SaveCanvas").GetComponent<CanvasGroup>();
        SetPanelState(startGroup, true);
        SetPanelState(newGroup,false);
        SetPanelState(savecanvas, false);
        SetPanelState(settingGroup,false);
        SetPanelState(exitGroup,false);
    }

    public void OnClickedNewOption()
    {
        SetPanelState(startGroup, false);
        SetPanelState(newGroup, true);
    }
    public void OnClickedSettingOption()
    {
        SetPanelState(settingGroup, true);
        SetPanelState(startGroup, false);
    }
    public void OnClickedLoadOption()
    {
        UIManager.Instance.OnLoadUI();
    }
    public void OnClickedExitOption()
    {
        SetPanelState(exitGroup, true);
        SetPanelState(startGroup, false);
    }

    public void ClosePanel()
    {
        savecanvas = GameObject.Find("SaveCanvas").GetComponent<CanvasGroup>();
        SetPanelState(startGroup, true);
        SetPanelState(settingGroup, false);
        SetPanelState(savecanvas, false);
        SetPanelState(exitGroup, false);
        
        Time.timeScale = 1;
    }

    public void StartTheDefaultCanvas()
    {
        SetPanelState(startGroup,true);
    }
    private void SetPanelState(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }
}
