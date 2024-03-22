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
    protected float _jumpHeight = 3f;
    bool runDown;


    bool swingDown;
    bool isSwingReady;
    float swingkDelay;

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
    protected virtual void Run()
    {
        runDown = Input.GetKey(KeyCode.LeftShift);
        if (runDown)
            _moveSpeed = _runSpeed;
        anim.SetBool("isRun", runDown && dir!=Vector3.zero);
    }
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
    protected void Dead()
    {
        Debug.Log("GameOver...");
    }

    // Robber
    void Attack()
    {
        if (Input.GetMouseButtonDown(0)) // Swing
        {
            swingkDelay += Time.deltaTime;
            isSwingReady = weapon.rate < swingkDelay;

            if (swingDown && isSwingReady)
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
