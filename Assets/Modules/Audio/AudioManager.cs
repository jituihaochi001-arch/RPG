using UnityEngine;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public AudioSource bgm;
    public AudioSource soundEffects;

    public Slider bgmSlider;
    public Slider effectsSlider;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // 加载保存的音量
        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            float savedBGMVolume = PlayerPrefs.GetFloat("bgmVolume");
            bgmSlider.value = savedBGMVolume;
            bgm.volume = savedBGMVolume;
        }
        // 加载音效音量
        if (PlayerPrefs.HasKey("effectsVolume"))
        {
            float savedEffectsVolume = PlayerPrefs.GetFloat("effectsVolume");
            effectsSlider.value = savedEffectsVolume;
            soundEffects.volume = savedEffectsVolume;
        }

            // 监听 Slider 值变化，代替 Update
            bgmSlider.onValueChanged.AddListener(OnSliderValueChanged);
            effectsSlider.onValueChanged.AddListener(OnEffectsVolumeChanged);
        
    }
    private void OnSliderValueChanged(float value)
    {
        bgm.volume = value;
        PlayerPrefs.SetFloat("bgmVolume", value);
        PlayerPrefs.Save();
    }
    private void OnEffectsVolumeChanged(float value)
    {
        soundEffects.volume = value;
        PlayerPrefs.SetFloat("effectsVolume", value);
        PlayerPrefs.Save();
    }


}