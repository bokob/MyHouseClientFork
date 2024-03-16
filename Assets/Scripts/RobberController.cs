using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 강도 컨트롤러
/// </summary>
public class RobberController : PlayerController
{

    /// <summary>
    /// 근접 공격과 관련된 변수
    /// </summary>
    bool swingDown;   // 마우스 왼쪽 키 눌렸는지
    bool isSwingReady;  // 공격 준비
    float swingkDelay; // 공격 딜레이

    protected Weapon weapon;

    void Start()
    {
    }

    void Update()
    {
        // PlayerConroller의 dir로 이동을 입력받는다.
        base.dir.x = Input.GetAxis("Horizontal");
        base.dir.z = Input.GetAxis("Vertical");
        base.dir = dir.normalized;

        base.Jump();
    }

    private void FixedUpdate()
    {
        base.Walk();
        base.Run();
        Attack();
    }

    /// <summary>
    /// 강도의 근접 공격 |
    /// 좌클릭: 휘두르기, 우클릭: 찌르기
    /// </summary>
    void Attack()
    {
        if (Input.GetMouseButtonDown(0)) // Swing
        {
            swingkDelay += Time.deltaTime;
            // isSwingReady = weapon.rate < swingkDelay; // 공격 딜레이로 공격 가능 여부 판단

            isSwingReady = true;

            //if (swingDown && isSwingReady) // 공격 가능한 상태
            //{
            //    weapon.Use();
            //    anim.SetTrigger("setTrigger");
            //    swingkDelay = 0;
            //}

            //if (isSwingReady) // 공격 가능한 상태
            //{
            //    //weapon.Use();
            //    anim.SetTrigger("setTrigger");
            //    swingkDelay = 0;
            //}
            anim.SetTrigger("setSwing");
            swingkDelay = 0;
        }
        else if (Input.GetMouseButtonDown(1)) // Stab
        {
            Debug.Log("Stap!");
            anim.SetTrigger("setStab");
        }
    }
}
