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

    public GameObject handMag;
    public GameObject gunMag;

    protected Weapon weapon;
    protected WeaponManager w_Manager;
    public Melee meleeWeapon;

    void Start()
    {
        handMag.SetActive(false);
        holdingGun = true;
        anim.SetBool("holdGun", holdingGun);
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
        Reload();
    }

    void HoldGun()
    {   
        if(gunMag.activeInHierarchy || handMag.activeInHierarchy)
        {
            holdingGun = true;
            anim.SetBool("holdGun", holdingGun);
        }
        else if(!gunMag.activeInHierarchy && !handMag.activeInHierarchy)
        {
            holdingGun = false;
            anim.SetBool("holdGun", holdingGun);
        }
    }

    protected void AimingGun()
    {
        isPressedAiming = Input.GetMouseButton(1);
        anim.SetBool("aimGun", isPressedAiming && holdingGun);
    }

    void Reload()
    {
        if(Input.GetKeyDown("r") && holdingGun)
        {
            anim.SetTrigger("reload");
            gunMag.SetActive(false);
            handMag.SetActive(true);
        }
    }

    void Attack()
    {
        if(holdingGun == true) return;

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

    void AfterReload()
    {
        gunMag.SetActive(true);
        handMag.SetActive(false);
    }

    
}
