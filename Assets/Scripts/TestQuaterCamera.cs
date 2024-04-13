using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQuaterCamera : MonoBehaviour
{
    [SerializeField]
    Vector3 _delta;

    [SerializeField]
    GameObject _player;

    private void LateUpdate()
    {
        transform.position = _player.transform.position + _delta;   // 카메라 위치
        transform.rotation = new Quaternion(50f, 0f, 0f, 0f);       // 사선 
        transform.LookAt(_player.transform);
    }
}
