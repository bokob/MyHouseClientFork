using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 한 손 근접 무기
/// </summary>
public class Melee : Weapon
{
    BoxCollider meleeArea;       // 근접 공격 범위
    TrailRenderer trailEffet;    // 휘두를 때 효과

    void Awake()
    {
        base.Type = Define.Type.Melee;
        meleeArea = gameObject.GetComponent<BoxCollider>();
        trailEffet = gameObject.GetComponentInChildren<TrailRenderer>();

        // TODO
        /*
         * 무기 능력치를 엑셀이나 json을 이용해 관리 예정
         * 따로 읽어와서 그 값들을 세팅해줘야 함
         * 현재 임시로 테스트를 위해 하드코딩 함
        */
        if (gameObject.tag == "Melee")
            base.Attack = 50;

    }

    /// <summary>
    /// Use() 실행하면서 Attack() 코루틴 같이 실행된다.
    /// </summary>
    public override void Use()
    {
        StopCoroutine("Attack");
        StartCoroutine("Attack");
    }

    /// <summary>
    /// 코루틴으로 Collider, TrailRenderer 특정 시간 동안만 활성화
    /// </summary>
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;
        trailEffet.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffet.enabled = false;
    }
}
