using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private bool isattackCooldown = false;
    private Rigidbody2D rb;
    private Transform player;
    private EnemyState enemyState;
    private int facingDirection = 1;  
    private Animator animator;

    private void Start()
    {  
        rb = GetComponent<Rigidbody2D>();
        animator = rb.GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }
    private void Update()
    {   //检测状态
        if (enemyState != EnemyState.Knockback)
        {
            CheckPlayer();

            if (isattackCooldown)
            {
                attackCooldown -= Time.deltaTime;
                if (attackCooldown < 0)
                {
                    isattackCooldown = false;
                    attackCooldown = 0f;
                }
            }
            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
    //追逐方法
    void Chase()
    {     
        if (player.position.x > rb.transform.position.x && facingDirection == -1 ||
               player.position.x < rb.transform.position.x && facingDirection == 1)
        {
            Flip();
        }
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }
    //检测攻击范围
    private void CheckPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position,playerDetectRange,playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;
            //检测是否在攻击范围内且攻击冷却已就绪
            if (Vector2.Distance(player.position, transform.position) <= attackRange && !isattackCooldown)
            {
                ChangeState(EnemyState.Attacking);
                isattackCooldown = true;
                attackCooldown = 2f;
            }
            else if (Vector2.Distance(player.position, transform.position) > attackRange && enemyState != EnemyState.Attacking )
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
      
    }
    
    //控制转向
    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
    }
    //切换状态
    public void ChangeState(EnemyState newState)
    {
        //退出当前动画
        if (enemyState == EnemyState.Idle) 
        {
            animator.SetBool("isIdle", false);
        }else if(enemyState == EnemyState.Chasing)
        {
            animator.SetBool("isChasing",false);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            animator.SetBool("isAttacking",false);
        }
        //更新当前状态
          enemyState = newState;
        //更新当前动画
        if (enemyState == EnemyState.Idle)
        {
            animator.SetBool("isIdle", true);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            animator.SetBool("isChasing", true);
        }
        else if(enemyState == EnemyState.Attacking)
        {
            animator.SetBool("isAttacking",true);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }
}
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback,
}
