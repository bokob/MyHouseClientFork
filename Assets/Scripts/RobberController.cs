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
    bool stabKeyDown;  // 마우스 왼쪽 키 눌렸는지
    bool isStabReady;  // 공격 준비
    float stabDelay;   // 공격 딜레이

    public Melee meleeWeapon; // 근접 무기

    void Start()
    {
    }

    void Update()
    {
        if (base._isDead) return;

        // PlayerConroller의 dir로 이동을 입력받는다.
        base.MoveKeyInput();

        base.Jump();
        base.Dead();
    }

    private void FixedUpdate()
    {
        if (base._isDead) return;

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
        swingKeyDown = Input.GetMouseButtonDown(0);
        stabKeyDown = Input.GetMouseButtonDown(1);

        if (meleeWeapon == null)
        {
            Debug.Log("현재 장착된 무기가 없음");
            return;
        }

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
