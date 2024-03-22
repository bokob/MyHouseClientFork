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
}
