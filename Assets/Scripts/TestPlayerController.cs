using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{

    protected float _walkSpeed = 5f;
    protected float _moveSpeed;

    // Animator anim;
    Rigidbody rb;
    Vector3 dir = Vector3.zero;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        // anim = GetComponent<Animator>();
    }

    private void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        dir = dir.normalized;

        // Jump();
    }

    private void FixedUpdate()
    {
        Walk();
    }

    protected virtual void Walk()
    {
        _moveSpeed = _walkSpeed;
        if(dir!=Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
            transform.position += dir * _moveSpeed * Time.deltaTime;
        }
        // anim.SetBool("isWalk", dir != Vector3.zero);
    }
    protected void Dead()
    {
        Debug.Log("GameOver...");
    }

}
