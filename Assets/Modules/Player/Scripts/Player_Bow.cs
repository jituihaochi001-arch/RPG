using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;

    public PlayerMovement playerMovement;

    private Vector2 aimDirection = Vector2.right;
    //冷却时间
    public float shootCooldown = 0.5f;
    private float shootTimer;
    //动画控制器
    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        shootTimer -= Time.deltaTime;

        HandleAiming();
        if (Input.GetButtonDown("Shoot") && shootTimer<=0 )
        {
            playerMovement.isShooting = true;
            animator.SetBool("isShooting", true);        
        }
    }
    private void OnEnable()
    {
        animator.SetLayerWeight(0,0);
        animator.SetLayerWeight(1,1);
    }
    private void OnDisable()
    {
        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 0);
    }


    //控制射击方向
    private void HandleAiming()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if(x != 0 || y != 0)
        {
            aimDirection = new Vector2(x, y).normalized;
            animator.SetFloat("aimX",aimDirection.x);
            animator.SetFloat("aimY",aimDirection.y);
        }
    }
    public void Shoot()
    {
        if (shootTimer<=0)
        {
            Arrow arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.direction = aimDirection;
            shootTimer = shootCooldown;

        }
        animator.SetBool("isShooting", false);
        playerMovement.isShooting = false;
    }
}
