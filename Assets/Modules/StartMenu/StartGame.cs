using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
   
        [SerializeField] private string gameSceneName = "RPG";
    
        /// 由按钮 OnClick 调用
        public void LoadGameScene()
        {
            // 开始新游戏：先把所有常驻运行时数据恢复到初始状态，再加载游戏场景
            NewGameInitializer.ResetAll();
            SceneManager.LoadScene(gameSceneName);
        }    
}
