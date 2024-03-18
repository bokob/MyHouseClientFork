using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 근접무기 로직만 담아버림 추후에 상속받을 수 있게 코드 수정해야 함
/// </summary>
public class Weapon : MonoBehaviour
{
    public Define.Type type;            // 무기 타입
    public int damage;                  // 공격력
    public float rate;                  // 공격 속도
    public BoxCollider meleeArea;       // 근접 공격 범위
    public TrailRenderer trailEffet;    // 휘두를 때 효과

    private void Awake()
    {
        rate = 1;
        type = Define.Type.Melee;
    }

    /// <summary>
    /// Use() 실행하면서 Swing() 코루틴이라 같이 실행된다.
    /// </summary>
    public void Use()
    {
        if(type == Define.Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        if(type == Define.Type.Range)
        {

        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;   
        trailEffet.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffet.enabled = false;
    }
}
