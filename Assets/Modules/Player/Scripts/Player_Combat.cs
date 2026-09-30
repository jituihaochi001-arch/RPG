using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;

    public Animator animator;
    public float cooldown = 0.5f;
    private float timer = 0f;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
    public void Attack()
    {
        if (timer <= 0)
        {
            animator.SetBool("isAttacking", true);        
            timer = cooldown;
        }
    }
    //对enemy造成伤害，通过动画机调用
    public void DealDamage()
    {
        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.WeaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            // 获取敌人组件
            EnemyHealth enemyHealth = enemies[0].GetComponent<EnemyHealth>();

            enemies[0].GetComponent<EnemyHealth>().ChangeHealth(-StatsManager.Instance.Damage);
            if (!enemyHealth.IsDead())
                enemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, StatsManager.Instance.KnockbackForce,StatsManager.Instance.KnockbackTime,StatsManager.Instance.StunTime);
        }
    }
    public void FinishAttacking()
    {
        animator.SetBool("isAttacking",false);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.WeaponRange);
    }
}
