using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    protected bool _isGround;  
    protected float _walkSpeed = 5f;
    protected float _runSpeed = 15f;
    protected float _moveSpeed;
    protected float _jumpHeight = 3f; // 점프 파워
    bool runDown; // 달리는 상태 판별


    // 공격 관련
    bool swingDown;   // 마우스 왼쪽 키 눌렸는지
    bool isSwingReady;  // 공격 준비
    float swingkDelay; // 공격 딜레이

    Animator anim;
    Rigidbody rb;
    Vector3 dir = Vector3.zero;

    Weapon weapon;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        dir = dir.normalized;

        Jump();
    }

    private void FixedUpdate()
    {
        Walk();
        Run();
        Attack();
    }

    protected virtual void Walk() // 이동
    {
        _moveSpeed = _walkSpeed;
        if(dir!=Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
            transform.position += dir * _moveSpeed * Time.deltaTime;
        }
        anim.SetBool("isWalk", dir != Vector3.zero); // 이동이 있으면 걷게
    }
    protected virtual void Run() // 달리는 속도로 만들기
    {
        runDown = Input.GetKey(KeyCode.LeftShift);
        if (runDown)
            _moveSpeed = _runSpeed;
        anim.SetBool("isRun", runDown && dir!=Vector3.zero);
    }
    protected void IsGround() // 땅인지 확인
    {
        Debug.DrawRay(transform.position + (Vector3.up * 0.2f), Vector3.down, Color.red);

        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.3f, layerMask))
            _isGround = true;
        else
            _isGround = false;
    }

    protected void Jump() // 점프
    {
        IsGround();
        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            Vector3 jumpPower = Vector3.up * _jumpHeight;
            rb.AddForce(jumpPower, ForceMode.VelocityChange); // ForceMode.VelocityChange는 질랴 무시하고 직접적으로 속도의 변화를 준다.
            anim.SetTrigger("setJump");
        }
    }
    protected void Dead()
    {
        Debug.Log("죽었닭...");
    }

    // Robber 전용 함수
    void Attack()
    {
        if (Input.GetMouseButtonDown(0)) // Swing
        {
            swingkDelay += Time.deltaTime;
            isSwingReady = weapon.rate < swingkDelay; // 공격 딜레이로 공격 가능 여부 판단

            if (swingDown && isSwingReady) // 공격 가능한 상태
            {
                weapon.Use();
                anim.SetTrigger("setTrigger");
                swingkDelay = 0;
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Stab
        {
            Debug.Log("Stap!");
            anim.SetTrigger("setStab");
        }
    }
}
