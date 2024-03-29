using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HomeownerController : PlayerController
{
    bool swingKeyDown;
    bool stabKeyDown;
    bool isStabReady;
    float swingDelay;
    float stabDelay;
    bool isSwingReady;
    bool holdingGun;
    bool isPressedAiming;

    protected Weapon weapon;
    public Melee meleeWeapon;
    void Start()
    {

    }
    void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        dir = dir.normalized;
        Jump();
        Dead();
    }
    void FixedUpdate()
    {
        if(base._isDead) return;
        
        Walk();
        Run();
        HoldGun();
        AimingGun();
        Attack();
    }

    void HoldGun()
    {
        if(Input.GetKeyDown("t") && !holdingGun)
        {
            holdingGun = true;
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

    void Attack()
    {
        swingKeyDown = Input.GetMouseButtonDown(0);
        stabKeyDown = Input.GetMouseButtonDown(1);

        

        swingDelay += Time.deltaTime;
        stabDelay += Time.deltaTime;
        isSwingReady = meleeWeapon.Rate < swingDelay; // 공격속도가 공격 딜레이보다 작으면 공격준비 완료
        isStabReady = meleeWeapon.Rate < stabDelay;

        if(swingKeyDown && isSwingReady && base._isGround) // 휘두르기
        {
            Debug.Log("시작");
            meleeWeapon.Use();
            anim.SetTrigger("setSwing");
            swingDelay = 0;
            swingKeyDown = false;
        }
        else if (stabKeyDown && isStabReady && base._isGround) // 찌르기
        {
            Debug.Log("시작");
            meleeWeapon.Use();
            anim.SetTrigger("setStab");
            stabDelay = 0;
            stabKeyDown = false;
        }
    }
}
