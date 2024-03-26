using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HomeownerController : PlayerController
{
    float swingkDealy;
    bool isSwingReady;
    bool holdingGun;
    bool holdingKnife;
    bool isPressedAiming;

    protected Weapon weapon;
    void Start()
    {

    }
    void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        dir = dir.normalized;
        Jump();
    }
    void FixedUpdate()
    {
        Walk();
        Run();
        HoldGun();
        AimingGun();
        HoldKnife();
        KnifeAttack();
        Dying();
    }

    void HoldGun()
    {
        if(Input.GetKeyDown("t") && !holdingGun)
        {
            holdingGun = true;
            holdingKnife = false;
            anim.SetBool("holdGun", holdingGun);
        }
        else if(Input.GetKeyDown("t") && holdingGun)
        {
            holdingGun = false;
            anim.SetBool("holdGun", holdingGun);
        }
    }

    void AimingGun()
    {
        isPressedAiming = Input.GetMouseButton(1);
        anim.SetBool("aimGun", isPressedAiming && holdingGun);
    }

    void HoldKnife()
    {
        if(Input.GetKeyDown("y") && !holdingKnife)
        {
            holdingKnife = true;
            holdingGun = false;
            anim.SetBool("holdKnife", holdingKnife);
        }
        else if(Input.GetKeyDown("y") && holdingKnife)
        {
            holdingKnife = false;
            anim.SetBool("holdKnife", holdingKnife);
        }
    }
    
    void KnifeAttack()
    {
        if(Input.GetMouseButtonDown(0) && holdingKnife)
        {
            swingkDealy += Time.deltaTime;
            isSwingReady = true;
            anim.SetTrigger("setSwing");
            swingkDealy = 0;
        }
        else if(Input.GetMouseButtonDown(1) && holdingKnife)
        {
            anim.SetTrigger("setStab");
        }
    }
    void Dying()
    {
        if(Input.GetKeyDown("u"))
        {
            anim.SetTrigger("Dead");
        }
    }
}
