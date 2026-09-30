using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
        enemy_Movement = GetComponent<Enemy_Movement>();
    }


    //·¢ËÍ»÷ÍËÖ¸Áî
    public void Knockback(Transform forceTransform, float knockbackForce,float knockbackTime,float stunTime)
    {
        enemy_Movement.ChangeState(EnemyState.Knockback);
        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.velocity = direction * knockbackForce;
        StartCoroutine(knockbackCounter(knockbackTime,stunTime));
    }
    IEnumerator knockbackCounter(float knockbackTime,float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime); 
        enemy_Movement.ChangeState(EnemyState.Idle);
    }
}
