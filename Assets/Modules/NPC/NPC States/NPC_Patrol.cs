using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC_Patrol : MonoBehaviour
{

    public Vector2[] patrolPoints;
    public float speed = 2f;

    public float pauseDuration = 1.5f;

    private bool isPaused;
    private int currentPatrolIndex;
    private Vector2 target;
    

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(SetPatrolPoint());
    }

   
    void Update()
    {
        if (isPaused)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 direction = (target- (Vector2)transform.position).normalized;
          if(direction.x<0 && transform.localScale.x >0 || direction.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x*(-1),transform.localScale.y,transform.localScale.z);
        }


            rb.velocity = direction * speed;
        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            StartCoroutine(SetPatrolPoint());
        }
    }
    //使用协程，到达目标点暂停一下再前往下一目标点
    IEnumerator SetPatrolPoint()
    {
        isPaused = true;
        anim.Play("Idle");

        yield return new WaitForSeconds(pauseDuration);

        currentPatrolIndex = (currentPatrolIndex + 1)% patrolPoints.Length;
        target = patrolPoints[currentPatrolIndex];
        isPaused = false;
        anim.Play("Walk");
    }


}
