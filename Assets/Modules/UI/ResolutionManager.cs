using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 屏幕适配、窗口分辨率和全屏/窗口化设置。
/// 1. 把场景里的 CanvasScaler 统一为 1920x1080 等比缩放；
/// 2. 绑定主界面设置面板里已有的“分辨率”TMP_Dropdown；
/// 3. 绑定“全屏/窗口”按钮，并用 PlayerPrefs 记住玩家选择。
///
/// 该对象通过 RuntimeInitializeOnLoadMethod 自动创建，不需要手动挂到场景或预制体上。
/// </summary>
public class ResolutionManager : MonoBehaviour
{
    private const string WidthKey = "screenResolutionWidth";
    private const string HeightKey = "screenResolutionHeight";
    private const string FullscreenKey = "screenFullscreen";

    private static ResolutionManager instance;

    [Serializable]
    public struct ResolutionOption
    {
        public int width;
        public int height;

        public string Label => $"{width} x {height}";
    }

    // 常见的 16:9 窗口分辨率；实际创建 UI 时会过滤掉超过桌面分辨率的选项
    private static readonly ResolutionOption[] AllOptions =
    {
        new ResolutionOption { width = 1280, height = 720 },
        new ResolutionOption { width = 1600, height = 900 },
        new ResolutionOption { width = 1920, height = 1080 },
        new ResolutionOption { width = 2560, height = 1440 },
        new ResolutionOption { width = 3840, height = 2160 },
    };

    private readonly List<ResolutionOption> availableOptions = new List<ResolutionOption>();
    private TMP_Dropdown resolutionDropdown;
    private bool isApplying;

    // ===== 自动创建常驻的屏幕管理器 =====
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;

        GameObject managerObject = new GameObject("ResolutionManager");
        DontDestroyOnLoad(managerObject);
        managerObject.AddComponent<ResolutionManager>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        BuildAvailableOptions();
        ApplySavedDisplayMode();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        ConfigureAllCanvasScalers();
        EnsureDisplaySettingsUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 新场景加载后重新适配所有 Canvas，并在主界面出现时补上设置绑定
        ConfigureAllCanvasScalers();
        EnsureDisplaySettingsUI();
    }

    // ===== 统一 Canvas 缩放模式 =====
    private void ConfigureAllCanvasScalers()
    {
        CanvasScaler[] scalers = Resources.FindObjectsOfTypeAll<CanvasScaler>();
        foreach (CanvasScaler scaler in scalers)
        {
            if (scaler == null) continue;

            // 过滤预制体资源，只处理当前场景里的 Canvas
            if (!scaler.gameObject.scene.IsValid()) continue;

            // 自定义光标画布使用屏幕像素定位，不能参与 CanvasScaler 缩放，否则光标会和鼠标错位
            if (scaler.gameObject.name.Contains("Cursor")) continue;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }
    }

    // ===== 绑定主界面设置面板里的分辨率下拉框和全屏/窗口按钮 =====
    private void EnsureDisplaySettingsUI()
    {
        SettingsPanel settingsPanel = FindObjectOfType<SettingsPanel>(true);
        if (settingsPanel == null) return;

        Transform settingRoot = settingsPanel.transform.Find("setting");
        if (settingRoot == null)
        {
            settingRoot = settingsPanel.transform;
        }

        BindResolutionDropdown(settingRoot);
        BindWindowModeButtons(settingRoot);
    }

    // ===== 找到并初始化“分辨率”下拉框 =====
    private void BindResolutionDropdown(Transform settingRoot)
    {
        TMP_Dropdown dropdown = FindResolutionDropdown(settingRoot);
        if (dropdown == null)
        {
            Debug.LogWarning("ResolutionManager: 没有在设置面板里找到分辨率下拉框。");
            return;
        }

        resolutionDropdown = dropdown;

        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();
        foreach (ResolutionOption option in availableOptions)
        {
            optionData.Add(new TMP_Dropdown.OptionData(option.Label));
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(optionData);
        resolutionDropdown.onValueChanged.RemoveAllListeners();

        int defaultIndex = GetDefaultOptionIndex();
        resolutionDropdown.value = defaultIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(OnResolutionSelected);

        // 关键修复：TMP_Dropdown 的 Template 如果保持激活，它的 Blocker 会覆盖全屏并拦截点击
        resolutionDropdown.Hide();
        if (resolutionDropdown.template != null)
        {
            resolutionDropdown.template.gameObject.SetActive(false);
        }
    }

    // 通过“分辨率”标签找到同一个设置行里的 TMP_Dropdown
    private TMP_Dropdown FindResolutionDropdown(Transform settingRoot)
    {
        TMP_Text[] labels = settingRoot.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text label in labels)
        {
            if (label == null) continue;

            string text = label.text;
            if (text.Contains("分辨率") || text.Contains("Resolution"))
            {
                Transform row = label.transform.parent;
                if (row != null)
                {
                    TMP_Dropdown dropdown = row.GetComponentInChildren<TMP_Dropdown>(true);
                    if (dropdown != null) return dropdown;
                }
            }
        }

        // 兜底：兼容还没有替换掉默认 1/2/3 选项的场景
        TMP_Dropdown[] dropdowns = settingRoot.GetComponentsInChildren<TMP_Dropdown>(true);
        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            if (IsResolutionOptionList(dropdown)) return dropdown;
        }

        return null;
    }

    private bool IsResolutionOptionList(TMP_Dropdown dropdown)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0) return false;

        bool looksLikePlaceholder = true;
        bool looksLikeResolution = true;

        foreach (TMP_Dropdown.OptionData option in dropdown.options)
        {
            string text = option.text;
            if (text != "1" && text != "2" && text != "3") looksLikePlaceholder = false;
            if (!text.Contains(" x ")) looksLikeResolution = false;
        }

        return looksLikePlaceholder || looksLikeResolution;
    }

    // ===== 绑定“全屏 / 窗口”按钮 =====
    private void BindWindowModeButtons(Transform settingRoot)
    {
        Button[] buttons = settingRoot.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button == null) continue;

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label == null) continue;

            string text = label.text;
            string lower = text.ToLowerInvariant();

            if (text.Contains("全屏") || lower.Contains("fullscreen"))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnFullscreenClicked);
            }
            else if (text.Contains("窗口") || lower.Contains("windowed"))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnWindowedClicked);
            }
        }
    }

    // ===== 分辨率选项 =====
    private void BuildAvailableOptions()
    {
        availableOptions.Clear();

        int desktopWidth = Screen.currentResolution.width;
        int desktopHeight = Screen.currentResolution.height;

        foreach (ResolutionOption option in AllOptions)
        {
            if (option.width <= desktopWidth && option.height <= desktopHeight)
            {
                availableOptions.Add(option);
            }
        }

        // 极端情况下桌面分辨率低于 1280x720，也至少保留一个档位
        if (availableOptions.Count == 0)
        {
            availableOptions.Add(AllOptions[0]);
        }
    }

    private int GetDefaultOptionIndex()
    {
        int savedWidth = PlayerPrefs.GetInt(WidthKey, 0);
        int savedHeight = PlayerPrefs.GetInt(HeightKey, 0);

        if (savedWidth > 0 && savedHeight > 0)
        {
            int savedIndex = FindOptionIndex(savedWidth, savedHeight);
            if (savedIndex >= 0) return savedIndex;
        }

        // 没有存档时优先匹配当前窗口分辨率
        int currentIndex = FindOptionIndex(Screen.width, Screen.height);
        if (currentIndex >= 0) return currentIndex;

        // 最后优先 1920x1080，否则使用当前桌面支持的最大档位
        int defaultIndex = FindOptionIndex(1920, 1080);
        return defaultIndex >= 0 ? defaultIndex : availableOptions.Count - 1;
    }

    private int FindOptionIndex(int width, int height)
    {
        for (int i = 0; i < availableOptions.Count; i++)
        {
            if (availableOptions[i].width == width && availableOptions[i].height == height)
            {
                return i;
            }
        }

        return -1;
    }

    // ===== 启动时恢复上一次的显示模式 =====
    private void ApplySavedDisplayMode()
    {
        if (PlayerPrefs.GetInt(FullscreenKey, 0) == 1)
        {
            ApplyFullscreen(false);
            return;
        }

        int savedWidth = PlayerPrefs.GetInt(WidthKey, 0);
        int savedHeight = PlayerPrefs.GetInt(HeightKey, 0);

        if (savedWidth > 0 && savedHeight > 0)
        {
            ApplyResolution(savedWidth, savedHeight, false);
        }
    }

    private void OnResolutionSelected(int index)
    {
        if (isApplying) return;
        if (index < 0 || index >= availableOptions.Count) return;

        ResolutionOption option = availableOptions[index];
        ApplyResolution(option.width, option.height, true);
    }

    private void OnFullscreenClicked()
    {
        ApplyFullscreen(true);
    }

    private void OnWindowedClicked()
    {
        int width = PlayerPrefs.GetInt(WidthKey, 1920);
        int height = PlayerPrefs.GetInt(HeightKey, 1080);
        ApplyResolution(width, height, true);
    }

    private void ApplyFullscreen(bool save)
    {
        isApplying = true;

        Resolution desktop = Screen.currentResolution;
        Screen.SetResolution(desktop.width, desktop.height, FullScreenMode.FullScreenWindow);

        if (save)
        {
            PlayerPrefs.SetInt(FullscreenKey, 1);
            PlayerPrefs.Save();
        }

        isApplying = false;
    }

    private void ApplyResolution(int width, int height, bool save)
    {
        isApplying = true;

        int desktopWidth = Screen.currentResolution.width;
        int desktopHeight = Screen.currentResolution.height;

        // 防止窗口尺寸超过当前桌面分辨率
        if (width > desktopWidth) width = desktopWidth;
        if (height > desktopHeight) height = desktopHeight;

        Screen.SetResolution(width, height, FullScreenMode.Windowed);

        if (save)
        {
            PlayerPrefs.SetInt(WidthKey, width);
            PlayerPrefs.SetInt(HeightKey, height);
            PlayerPrefs.SetInt(FullscreenKey, 0);
            PlayerPrefs.Save();
        }

        isApplying = false;
    }
}
