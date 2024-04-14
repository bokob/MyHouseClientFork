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

    private void Awake()
    {
        
    }

    void LateUpdate()
    {
        QuterView();
    }


    void QuterView()
    {
        transform.position = _player.transform.position + _delta;   // 카메라 위치
        transform.rotation = new Quaternion(50f, 0f, 0f, 0f);       // 사선 
        transform.LookAt(_player.transform);
    }

    void TPSView()
    {

    }
}
