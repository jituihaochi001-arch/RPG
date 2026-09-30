using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerData player = new PlayerData();
    public StatsData stats = new StatsData();
    public SkillTreeData skillTree = new SkillTreeData();  
    public QuestSaveData quests = new QuestSaveData(); 
    public InventorySaveData inventory = new InventorySaveData();
  

}
   

[System.Serializable]
public class PlayerData
{

    public string currentScene = "";
    
    public float posX = 0f;
    public float posY = 0f;
    public float posZ = 0f;

    public int exp = 0;
    public int level = 0;
}

