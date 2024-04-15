using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;



public class Enemy_ : MonoBehaviour
{
    public Transform target; // 玩家角色的Transform
    public Transform patrolPoint1; // 第一个巡逻点
    public Transform patrolPoint2; // 第二个巡逻点
    public float patrolSpeed = 3f; // 巡逻速度
    public float returnToOriginalDistance = 1f; // 返回原点距离
    public float chaseRange = 10f; // 追逐范围
    public float attackRange = 2f; // 攻击范围
    public float attackDelay = 2f; // 攻击间隔

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 originalPosition;
    private Transform currentPatrolPoint;
    private float nextAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 保存敌人的原始位置
        originalPosition = transform.position;

        // 初始时，设置第一个巡逻点为当前目标点
        currentPatrolPoint = patrolPoint1;
    }

    void Update()
    {
        // 计算敌人与当前巡逻点的距离
        float distanceToPatrolPoint = Vector3.Distance(transform.position, currentPatrolPoint.position);

        // 如果与巡逻点距离小于某个阈值，则切换巡逻点
        if (distanceToPatrolPoint < 1f)
        {
            // 切换到另一个巡逻点
            if (currentPatrolPoint == patrolPoint1)
                currentPatrolPoint = patrolPoint2;
            else
                currentPatrolPoint = patrolPoint1;
        }

        // 计算敌人与玩家的距离
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= chaseRange)
        {
            // 玩家进入追逐范围，停止巡逻并追击玩家
            agent.SetDestination(target.position);

            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                // 在攻击范围内，进行攻击
                Attack();
            }
            else
            {
                // 不在攻击范围内，停止攻击动画
                SetIsAttacking(false);
            }
        }
        else
        {
            // 玩家脱离追逐范围，继续巡逻
            agent.SetDestination(currentPatrolPoint.position);

            // 移动动画
            SetIsMoving(true);

            // 攻击动画
            SetIsAttacking(false);
        }
    }

    void Attack()
    {
        // 进行攻击逻辑
        Debug.Log("Attack!");

        // 攻击动画
        SetIsMoving(false);
        SetIsAttacking(true);

        // 设置下一次攻击的时间
        nextAttackTime = Time.time + attackDelay;
    }

    void SetIsMoving(bool isMoving)
    {
        // 设置Animator的bool参数
        animator.SetBool("IsMoving", isMoving);
    }

    void SetIsAttacking(bool isAttacking)
    {
        // 设置Animator的bool参数
        animator.SetBool("IsAttacking", isAttacking);
    }
}
