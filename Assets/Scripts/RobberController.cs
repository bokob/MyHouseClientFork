using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberController : MonoBehaviour
{
    Animator anim;
    private Vector3 dir = Vector3.zero;

    new void Start()
    {
        //base.Start();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Idle();
        Run();
        Walk();
        Jump();
        Attack();
    }

    void Idle()
    {
        bool isPlayingAnim = anim.GetCurrentAnimatorStateInfo(0).length > anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (!Input.anyKey && !isPlayingAnim)
        {
            anim.Play("Idle");
        }
    }

    new void Walk()
    {
        //base.Walk();
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            anim.Play("Walk");
    }

    new void Run()
    {
        //base.Run();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.Play("Run");
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
            anim.SetTrigger("setSwing");

        if(Input.GetMouseButtonDown(1))
            anim.SetTrigger("setStab");
    }

    new void Jump()
    {
        //IsGround();
        //if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        //{
        //    base.Jump();
        //    anim.SetTrigger("setJump");
        //}
    }
}
