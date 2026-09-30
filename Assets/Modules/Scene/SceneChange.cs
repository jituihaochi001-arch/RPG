using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{

    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 0.5f;

    public Vector2 newPlayerPosition;
    private Transform player;

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.gameObject.tag == "Player")
        {
            
            // 先把玩家位置存到 GameManager 里
            GameManager.Instance.SetPlayerStartPosition(newPlayerPosition);          
            //播放淡出动画
            fadeAnim.Play("FadeToWhite");
            StartCoroutine(DelayFade());                       
        }
    }
    
    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
      
        SceneManager.LoadScene(sceneToLoad);    
    }
   
    
  
}
