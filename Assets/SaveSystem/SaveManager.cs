using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveDirectory;


    // 当前选中的存档位（默认 0）
    private int currentSlotIndex = 0;

    // =====存储待恢复的存档数据 =====
    public SaveData pendingSaveData = null;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            saveDirectory = Application.persistentDataPath;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;//订阅事件         
        }
        else if (Instance != this)
        {           
            Destroy(gameObject);
            return;
        }
        
    }
    private void OnDestroy()
    {        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ===== 获取存档路径 =====
    private string GetSavePath(int slotIndex)
    {
        return Path.Combine(saveDirectory, $"save_{slotIndex}.json");
    }

    // ===== 保存到指定存档位 =====
    public void SaveGame(int slotIndex)
    {
        string savePath = GetSavePath(slotIndex);

        SaveData saveData = new SaveData();

        // 保存当前场景名
        saveData.player.currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"保存当前场景：{saveData.player.currentScene}");
        //保存玩家位置数据
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            //保存玩家经验值
            saveData.player.exp = ExpManager.Instance.currentExp;
            saveData.player.level = ExpManager.Instance.level;

            saveData.player.posX = player.transform.position.x;
            saveData.player.posY = player.transform.position.y;
            saveData.player.posZ = player.transform.position.z;
            Debug.Log($"保存玩家位置: ({saveData.player.posX}, {saveData.player.posY}, {saveData.player.posZ})");
        }
        else
        {
            Debug.LogWarning("找不到 Player，位置数据保存为默认值 0");
        }
        //保存玩家属性
        saveData.stats = StatsManager.Instance.GetSaveData();
        //保存技能树进度
        saveData.skillTree = SkillTreeManager.Instance.GetSaveData();
        //保存背包
        saveData.inventory = InventoryManager.Instance.GetSaveData();
        //保存任务
        saveData.quests = QuestManager.Instance.GetSaveData();

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"游戏已保存到存档位 {slotIndex + 1}");
    }

    
    // ===== 从指定存档位加载 =====
    public void LoadGame(int slotIndex)
    {
       
        string savePath = GetSavePath(slotIndex);

        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"存档位 {slotIndex + 1} 没有存档");
            return;
        }

        string json = File.ReadAllText(savePath);
        pendingSaveData = JsonUtility.FromJson<SaveData>(json);

   
        // 先加载存档记录的场景
        StartCoroutine(LoadProgress(pendingSaveData.player.currentScene));      

        currentSlotIndex = slotIndex;
    

        Debug.Log($"游戏已从存档位 {slotIndex + 1} 加载");
    }

    // ===== 场景加载完成后自动触发 =====
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pendingSaveData == null) return;

        // 检查是否加载的是存档记录的场景
        if (scene.name == pendingSaveData.player.currentScene)
        {          
            // 恢复玩家位置
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                // 恢复数据到管理器            
                StatsManager.Instance.LoadSaveData(pendingSaveData.stats);
                SkillTreeManager.Instance.LoadSaveData(pendingSaveData.skillTree);
                QuestManager.Instance.LoadSaveData(pendingSaveData.quests);
                InventoryManager.Instance.LoadSaveData(pendingSaveData.inventory);

                ExpManager.Instance.currentExp = pendingSaveData.player.exp;
                ExpManager.Instance.level = pendingSaveData.player.level;

                player.transform.position = new Vector3(
                    pendingSaveData.player.posX,
                    pendingSaveData.player.posY,
                    pendingSaveData.player.posZ
                );
                Debug.Log($"玩家位置已恢复");
            }

            pendingSaveData = null;
         
        }
    }
  
    // ===== 检查指定存档位是否有存档 =====
    public bool HasSave(int slotIndex)
    {
        return File.Exists(GetSavePath(slotIndex));
    }

    // ===== 检查当前存档位是否有存档 =====
    public bool HasSave()
    {
        return HasSave(currentSlotIndex);
    }

    // ===== 删除指定存档位 =====
    public void DeleteSave(int slotIndex)
    {
        string savePath = GetSavePath(slotIndex);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"存档位 {slotIndex + 1} 已删除");
        }
    }

    // ===== 获取所有有存档的索引 =====
    public List<int> GetSaveSlots()
    {
        List<int> slots = new List<int>();
        for (int i = 0; i < 4; i++) 
        {
            if (File.Exists(GetSavePath(i)))
            {
                slots.Add(i);
            }
        }
        return slots;
    }
    // ===== 获取存档的时间 =====
    public string GetSaveTimestamp(int slotIndex)
    {
        string savePath = GetSavePath(slotIndex);
        if (!File.Exists(savePath)) return "";

        // 获取文件的最后修改时间
        DateTime lastWriteTime = File.GetLastWriteTime(savePath);
        return lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    #region 异步加载动画
    public GameObject loadScreen;
    public Slider slider;
    public TMP_Text percent;

    IEnumerator LoadProgress(string sceneName)
    {

        loadScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            percent.text = (progress * 100).ToString("F0") + "%";
            yield return null;
        }
        slider.value = 1f;
        percent.text = "100%";
        loadScreen.SetActive(false);
    }
    #endregion
}