using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� �� ���� ����
/// </summary>
public class Melee : Weapon
{
    BoxCollider meleeArea;       // ���� ���� ����
    TrailRenderer trailEffet;    // �ֵθ� �� ȿ��

    void Awake()
    {
        base.Type = Define.Type.Melee;
        meleeArea = gameObject.GetComponent<BoxCollider>();
        trailEffet = gameObject.GetComponentInChildren<TrailRenderer>();
    }

    /// <summary>
    /// Use() �����ϸ鼭 Attack() �ڷ�ƾ ���� ����ȴ�.
    /// </summary>
    public override void Use()
    {
        StopCoroutine("Attack");
        StartCoroutine("Attack");
    }

    /// <summary>
    /// �ڷ�ƾ���� Collider, TrailRenderer Ư�� �ð� ���ȸ� Ȱ��ȭ
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
