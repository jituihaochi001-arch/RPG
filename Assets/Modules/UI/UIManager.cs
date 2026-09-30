using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private CanvasGroup menuBar;
    private bool isMenuActive;

    [SerializeField] private CanvasGroup statsMenu;
    [SerializeField] private CanvasGroup skillsMenu;
    [SerializeField] private CanvasGroup questsMenu;
    [SerializeField] private CanvasGroup settingMenu;
    [SerializeField] private CanvasGroup audioMenu;
    [SerializeField] private CanvasGroup detailMenu;
    [SerializeField] private CanvasGroup saveMenu;

    [SerializeField] private CanvasGroup[] gameCanvas;

    [SerializeField] private Image menueToggleImage;

    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;
    //保存/读取界面标题
     public bool isSave;
    [SerializeField] private TMP_Text topic;
    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        foreach (CanvasGroup cg in gameCanvas)
        {
            if (cg != null)
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
        
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
            ClosePanel();

    }
    // 在场景切换时调用
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndToggleCanvasGroups(scene.name);
    }
    public void CheckAndToggleCanvasGroups(string sceneName)
    {
        bool isStartScene = sceneName == "Start Scene";
    
            foreach (CanvasGroup cg in gameCanvas)
            {
                if (cg != null)
                {
                    cg.alpha = isStartScene ? 0 : 1;
                    cg.interactable = !isStartScene;
                    cg.blocksRaycasts = !isStartScene;
                }
            }
        SetMenuState(saveMenu,false);
        SetMenuState(settingMenu,false);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void ClosePanel()
    {
        
            SetMenuState(settingMenu, false);
            SetMenuState(statsMenu, false);
            SetMenuState(skillsMenu, false);
            SetMenuState(questsMenu, false);
            SetMenuState(audioMenu, false);
            SetMenuState(detailMenu, false);
            SetMenuState(saveMenu, false);

            Time.timeScale = 1;
        
    }

    public void ToggleMenu(CanvasGroup target)
    {
        SetMenuState(statsMenu, false);
        SetMenuState(skillsMenu, false);
        SetMenuState(questsMenu, false);
        SetMenuState(settingMenu, false);

        SetMenuState(target, true);
    }

    public  void ToggleMainMenu()
    {
        isMenuActive = !isMenuActive;
        SetMenuState(menuBar, isMenuActive);
        menueToggleImage.sprite = isMenuActive ? closeSprite : openSprite;

        SetMenuState(statsMenu, false);
        SetMenuState(skillsMenu, false);
        SetMenuState(questsMenu, false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnSettingButtonClicked()
    {
        SetMenuState(settingMenu, true);
        Time.timeScale = 0;
        
    }
    public void OnAudioButtonClicked()
    {
        SetMenuState(audioMenu, true);
        Time.timeScale = 0;
        
    }
    public void OnDetailButtonClicked()
    {
        SetMenuState(detailMenu, true);
        Time.timeScale = 0;
        
    }

    public void OnSaveUI()
    {
        ClosePanel();
        Time.timeScale = 0;
        isSave = true;
        topic.text = "保存游戏";
        SetMenuState(saveMenu, true);       
    }
    public void OnLoadUI()
    {
        ClosePanel();
        Time.timeScale = 0;
        isSave = false;
        topic.text = "读取游戏";
        SetMenuState(saveMenu, true);
    }

    private void SetMenuState(CanvasGroup group,bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }
    public void ReturnMainMenu()
    {
        Debug.Log("回到主菜单");
        Time.timeScale = 1;
        SceneManager.LoadScene("Start Scene");
    }


}
