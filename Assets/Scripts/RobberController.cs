using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��Ʈ�ѷ�
/// </summary>
public class RobberController : PlayerController
{
    /// <summary>
    /// ���� ���ݰ� ���õ� ����
    /// </summary>
    bool swingKeyDown;  // ���콺 ���� Ű ���ȴ���
    bool isSwingReady;  // ���� �غ�
    float swingDelay;   // ���� ������
    bool stabKeyDown;  // ���콺 ���� Ű ���ȴ���
    bool isStabReady;  // ���� �غ�
    float stabDelay;   // ���� ������

    public Melee meleeWeapon; // ���� ����

    void Start()
    {
    }

    void Update()
    {
        if (base._isDead) return;

        // PlayerConroller�� dir�� �̵��� �Է¹޴´�.
        base.dir.x = Input.GetAxis("Horizontal");
        base.dir.z = Input.GetAxis("Vertical");
        base.dir = dir.normalized;

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
    /// ������ ���� ���� |
    /// ��Ŭ��: �ֵθ���, ��Ŭ��: ���
    /// </summary>
    void Attack()
    {
        swingKeyDown = Input.GetMouseButtonDown(0);
        stabKeyDown = Input.GetMouseButtonDown(1);

        if (meleeWeapon == null)
        {
            Debug.Log("���� ������ ���Ⱑ ����");
            return;
        }

        swingDelay += Time.deltaTime;
        stabDelay += Time.deltaTime;
        isSwingReady = meleeWeapon.Rate < swingDelay; // ���ݼӵ��� ���� �����̺��� ������ �����غ� �Ϸ�
        isStabReady = meleeWeapon.Rate < stabDelay;

        if(swingKeyDown && isSwingReady && base._isGround) // �ֵθ���
        {
            Debug.Log("����");
            meleeWeapon.Use();
            anim.SetTrigger("setSwing");
            swingDelay = 0;
            swingKeyDown = false;
        }
        else if (stabKeyDown && isStabReady && base._isGround) // ���
        {
            Debug.Log("����");
            meleeWeapon.Use();
            anim.SetTrigger("setStab");
            stabDelay = 0;
            stabKeyDown = false;
        }
    }
}
