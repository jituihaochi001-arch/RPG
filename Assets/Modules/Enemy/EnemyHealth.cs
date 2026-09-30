using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyHealth : MonoBehaviour
{   
    public int ExpReward = 2;
    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    public int currentHealth;
    public int maxHealth;

    public Animator anim;

    private void OnEnable()
    {
        currentHealth = maxHealth;  // 从池中取出时重置血量
        anim.Play("Idle");       
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {            
            Die();
        }
    }

    private void Die()
    {   //死亡时取消所有协程
        StopAllCoroutines();

        // 触发死亡事件（供特效、任务等订阅）
        OnMonsterDefeated?.Invoke(ExpReward);

        //播放死亡动画
        anim.Play("Die");
    }
    public void OnDeadAnimationComplete()
    {
        EnemyPool.Instance.Release(gameObject);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;   // ← 实时计算当前是否死亡
    }
}
