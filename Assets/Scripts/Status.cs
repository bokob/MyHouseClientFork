using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public float Hp { get; private set; } = 100;    // ü��
    public float Sp { get; private set; } = 100;    // ���׹̳�
    public float MaxHp { get; private set; } = 100; // �ִ� ü��
    public float MaxSp { get; private set; } = 100; // �ִ� ���׹̳�
    public float Defence { get; private set; } = 1; // ����

    /// <summary>
    /// ������ ó�� �Լ�
    /// </summary>
    public void TakedDamage(int damage)
    {
        // TODO
        /*
        ���� = ���� - ������
        1. ���ذ� ������� ȸ���Ǵ� ������ �Ͼ�Ƿ� ������ ���� 0�̻����� �ǰԲ� �����ؾ� �Ѵ�.
        2. ü�¿��� ���ظ� ����.
        3. ü���� 0���ϰ� �Ǹ� PlayerConroller�� �ִ� isDead�� true�� �ٲ۴�.
         * 
        */
    }
}
