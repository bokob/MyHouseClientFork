using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public float Hp { get; private set; } = 100;    // 체력
    public float Sp { get; private set; } = 100;    // 스테미나
    public float MaxHp { get; private set; } = 100; // 최대 체력
    public float MaxSp { get; private set; } = 100; // 최대 스테미나
    public float Defence { get; private set; } = 1; // 방어력

    /// <summary>
    /// 데미지 처리 함수
    /// </summary>
    public void TakedDamage(int attack)
    {
        // 피해가 음수라면 회복되는 현상이 일어나므로 피해의 값을 0이상으로 되게끔 설정
        float damage = Mathf.Max(0, attack - Defence);
        Hp -= damage;

        Debug.Log(gameObject.name + "(이)가 " + damage + " 만큼 피해를 입었음!");
    }
}
