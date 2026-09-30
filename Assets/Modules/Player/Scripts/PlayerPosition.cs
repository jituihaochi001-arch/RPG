using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    void Start()
    {
        // 如果 GameManager 存了起始位置，就移动过去
        if (GameManager.Instance.HasPlayerStartPosition())
        {
            Vector2 startPos = GameManager.Instance.GetPlayerStartPosition();
            transform.position = startPos;
        }
    }

}
