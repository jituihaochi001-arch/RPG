using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player_Combat player_Combat;
    public int facingDirection = 1;
    public Rigidbody2D rb;
    public Animator anim;
    public bool isKnockedBack;
    public bool isShooting;
    private void Update()
    {
        if (Input.GetButtonDown("Slash") && player_Combat.enabled == true)
        {
            player_Combat.Attack();
        }
    }
    //控制人物移动
    void FixedUpdate()
    {
        if (isShooting == true)
        {
            rb.velocity = Vector2.zero;
        }
        else if (isKnockedBack == false)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (horizontal > 0 && transform.localScale.x < 0 ||
                horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }
            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));
            rb.velocity = new Vector2(horizontal, vertical) * StatsManager.Instance.Speed;
        }
    }
    //控制转向
    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
    }
    //设置被击退方法
    public void Knockback(Transform enemy,float force,float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        StartCoroutine(knockbackCounter(stunTime));
     }
    IEnumerator knockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }


}
