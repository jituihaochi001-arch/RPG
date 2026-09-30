using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_combat : MonoBehaviour
{
    public float knockbackForce;
    public float stunTime;
    public Transform attackPoint;
    public int damage = 1;
    public float weaponRange;
    public LayerMask playerLayer;
   
    public void  Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position,weaponRange,playerLayer);
        if (hits.Length > 0)
        {
            StatsManager.Instance.ChangeHealth(-damage);
            hits[0].GetComponent<PlayerMovement>().Knockback(transform,knockbackForce,stunTime);
        }
    }
}
