using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라, 현재는 쿼터뷰만 가능하게 되어있음
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField]
    Vector3 _delta;

    [SerializeField]
    GameObject _player;

    void LateUpdate()
    {
        transform.position = _player.transform.position + _delta;
        transform.LookAt(_player.transform);
    }
}
