using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorCtrl : MonoBehaviour
{
   public static CursorCtrl Instance { get; private set; }

    [Header("音频设置")]
    [SerializeField] private AudioClip clickSound;  // 拖入你的音频文件
    [SerializeField] private AudioSource audioSource;  // 拖入 AudioSource 组件
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0; // 左键

    private void Awake()
    {
         if(Instance == null)
            {
                 Instance = this;
            }
        else
            {
                 Destroy(gameObject);
             }

        // ===== 注册为持久化对象 =====
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPersistentObject(transform.parent.gameObject);
            GameManager.Instance.RegisterPersistentObject(gameObject); 
          
        }
    }
    void Start()
    {
        // 自定义光标只负责显示，关闭 Graphic 的射线检测，避免挡住下拉框等 UI 的点击
        foreach (Graphic graphic in GetComponentsInChildren<Graphic>(true))
        {
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }
        }

        // 把光标所在的 Canvas 放到最上层，保证下拉框展开时不会盖住鼠标图标
        Canvas cursorCanvas = GetComponentInParent<Canvas>();
        if (cursorCanvas != null)
        {
            cursorCanvas.sortingOrder = 999;
        }

        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        gameObject.transform.position = Input.mousePosition;
        if (Input.GetKeyDown(mouseButton))
        {
            PlayClickSound();
        }
    }

    private void PlayClickSound()
    {
        if (audioSource == null)
        {                                  
            audioSource = gameObject.AddComponent<AudioSource>();            
        }

        if (clickSound == null)
        {
            Debug.LogWarning("没有设置点击音效！");
            return;
        }
        audioSource.PlayOneShot(clickSound);
    }
}
