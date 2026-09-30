using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //唯一单例：游戏管理器
    public static GameManager Instance;
   
    public DialogueHistoryTracker DialogueHistoryTracker;
    public LocationHistoryTracker LocationHistoryTracker;

    // ===== 动态注册的持久化对象列表 =====
    [Header("PersistentObjects")]
    [SerializeField] private List<GameObject> persistentObjects = new List<GameObject>();

    // 存储玩家在新场景的初始位置
    private Vector2 playerStartPosition;
    private bool hasPlayerStartPosition = false;


    private void Awake()
    {
        if(Instance != null)
        {        
            Destroy(Instance);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);          
        }
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }
    
    // ===== 公开注册方法：任何对象都可以调用 =====
    public void RegisterPersistentObject(GameObject obj)
    {
        if (obj == null) return;

        // 避免重复注册同一个对象
        if (!persistentObjects.Contains(obj))
        {
            persistentObjects.Add(obj);
            DontDestroyOnLoad (obj);
        }
       
    }
  
    // ===== 公开移除方法（可选） =====
    public void UnregisterPersistentObject(GameObject obj)
    {
        if (persistentObjects.Contains(obj))
        {
            persistentObjects.Remove(obj);
        }
    }
  
    // 设置玩家初始位置（由 SceneChange 调用）
    public void SetPlayerStartPosition(Vector2 position)
    {
        playerStartPosition = position;
        hasPlayerStartPosition = true;
    }
    // 获取玩家初始位置（由新场景里的 Player 调用）
    public Vector2 GetPlayerStartPosition()
    {
        hasPlayerStartPosition = false;
        return playerStartPosition;
    }

    public bool HasPlayerStartPosition()
    {
        return hasPlayerStartPosition;
    }
}
