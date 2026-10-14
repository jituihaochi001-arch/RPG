using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocationHistoryTracker : MonoBehaviour
{   
    private readonly HashSet<LocationSO> locationsVisited = new HashSet<LocationSO>();
   

    public void RecordLocation(LocationSO locationSO)
    {
        if(locationsVisited.Add(locationSO))
            Debug.Log("just visited to " + locationSO.displayName);
    }


    public bool HasVisited(LocationSO locationSO)
    {
        return locationsVisited.Contains(locationSO);
    }

    // ===== 新游戏时清空地点访问历史 =====
    public void Clear()
    {
        locationsVisited.Clear();
    }
}

