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
    bool swingKeyDown;  // 마우스 왼쪽 키 눌렸는지
    bool isSwingReady;  // 공격 준비
    float swingDelay;   // 공격 딜레이

    public Weapon weapon;

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
        swingKeyDown = Input.GetMouseButton(0);

        if (weapon == null)
        {
            Debug.Log("널이라 무기가 없음");
            return;
        }

        swingDelay += Time.deltaTime;

        isSwingReady = weapon.rate < swingDelay; // 공격속도가 공격 딜레이보다 작으면 공격준비 완료

        if(swingKeyDown && isSwingReady && base._isGround)
        {
            Debug.Log("시작");
            weapon.Use();
            anim.SetTrigger("setSwing");
            swingDelay = 0;
            swingKeyDown = false;
        }
        //else if (Input.GetMouseButtonDown(1)) // Stab
        //{
        //    Debug.Log("Stap!");
        //    anim.SetTrigger("setStab");
        //}
    }
}
