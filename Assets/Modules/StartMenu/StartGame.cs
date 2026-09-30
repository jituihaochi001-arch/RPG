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
            SceneManager.LoadScene(gameSceneName);
        }    
}
