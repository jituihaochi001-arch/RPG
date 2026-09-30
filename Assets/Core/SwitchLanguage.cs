using UnityEngine;
using UnityEngine.Localization.Settings;


public class SwitchLanguage : MonoBehaviour
{
    [Header("Language Settings")]    
    [SerializeField] private string localeIdentifier = "zh-Hans";

    /// <summary>
    /// 切换当前游戏语言到指定的 Locale。
    /// 此方法可由 UI 按钮的 OnClick 事件调用。
    /// </summary>
    public void SwitchTheLanguage()
    {
        // 获取当前可用的所有语言（Locale）
        var availableLocales = LocalizationSettings.AvailableLocales.Locales;

        // 在可用语言列表中查找与 localeIdentifier 匹配的语言
        foreach (var locale in availableLocales)
        {
            if (locale.Identifier.Code == localeIdentifier)
            {
                // 找到匹配的语言，执行切换
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log($"语言已切换至: {locale.Identifier.Code}");
                return;
            }
        }

        // 如果遍历完都没找到匹配的语言，输出警告
        Debug.LogWarning($"未找到语言标识符为 '{localeIdentifier}' 的 Locale，请检查输入是否正确。");
    }
}