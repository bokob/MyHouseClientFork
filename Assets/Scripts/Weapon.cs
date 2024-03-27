using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ٰŸ�, ���Ÿ� ������� ��ӹ޴� �Ϲ�ȭ�� ���� Ŭ����
/// </summary>
public class Weapon : MonoBehaviour
{
    public Define.Type Type { get; protected set; } // ���� Ÿ��

    public int Damage { get; protected set; }       // ���ݷ�
    public float Rate { get; protected set; }       // ���ݼӵ�


    void Awake()
    {
        // TODO
        /*
         ���Ⱑ �پ����� �� ���� �̸��̳� Ÿ�Կ� ����
         �������� ���ݼӵ��� �����ϴ� �۾��� ����� ��
         */
    }

    /// <summary>
    /// Use() �����ϸ鼭 �� ���⿡ �´� ���� ȿ�� �ڷ�ƾ�� ���� ����ȴ�.
    /// </summary>
    public virtual void Use()
    {
        // TODO
        // ���⿡ �´� ���� ���
    }
}
