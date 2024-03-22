using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어(집주인, 강도)가 공통적으로 상속받는 클래스
/// </summary>
public class PlayerController : MonoBehaviour
{
    protected Rigidbody rb;
    protected Animator anim;

    protected Vector3 dir = Vector3.zero;
    protected bool _isGround;  
    protected float _walkSpeed = 5f;
    protected float _runSpeed = 15f;
    protected float _moveSpeed;
    protected float _jumpHeight = 3f; // 점프 파워
    bool isPressedRunKey; // 달리는 상태 판별
    
    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 기본 이동, 걷는 애니메이션 재생
    /// </summary>
    protected virtual void Walk()
    {
        _moveSpeed = _walkSpeed;
        if(dir!=Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
            transform.position += dir * _moveSpeed * Time.deltaTime;
        }
        anim.SetBool("isWalk", dir != Vector3.zero);
    }

    /// <summary>
    /// 달리기, 이동 속도를 변화시키고 달리는 애니메이션 재생
    /// </summary>
    protected virtual void Run() // 달리는 속도로 만들기
    {
        isPressedRunKey = Input.GetKey(KeyCode.LeftShift);
        if (isPressedRunKey)
            _moveSpeed = _runSpeed;
        anim.SetBool("isRun", isPressedRunKey && dir!=Vector3.zero);
    }

    /// <summary>
    /// Ground인지 판단
    /// </summary>
    protected void IsGround()
    {
        Debug.DrawRay(transform.position + (Vector3.up * 0.2f), Vector3.down, Color.red);

        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.3f, layerMask))
            _isGround = true;
        else
            _isGround = false;
    }

    /// <summary>
    /// 점프
    /// </summary>
    protected void Jump()
    {
        IsGround();
        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            Vector3 jumpPower = Vector3.up * _jumpHeight;
            rb.AddForce(jumpPower, ForceMode.VelocityChange);
            anim.SetTrigger("setJump");
        }
    }

    /// <summary>
    /// hp가 0이되면 사망
    /// </summary>
    protected void Dead()
    {
        Debug.Log("GameOver...");
    }
}
