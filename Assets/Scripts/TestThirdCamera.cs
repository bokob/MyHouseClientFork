using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class TestThirdCamera : MonoBehaviour
{
    [SerializeField]
    float _mouseSensitivity = 3.0f;

    private float _rotationY;
    private float _rotationX;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _distanceFromTarget = 3.0f;

    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private float _smoothTime = 0.2f;

    [SerializeField]
    private Vector2 _rotationXMinMax = new Vector2(-40, 40);

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        _rotationY += mouseX;
        _rotationX += mouseY;

        // 위 아래 각도 제한
        _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

        Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

        _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
        transform.localEulerAngles = _currentRotation;

        // 목표 지점에서 카메라의 위치를 빼고 목표까지의 거리를 곱해서 그만큼 떨어직게 위치시킨다.
        transform.position = _target.position - transform.forward * _distanceFromTarget;
    }






























    //[Header("Player 관련")]
    //GameObject _player;
    //Transform _playerTransform;

    //// 시점
    //[SerializeField]
    //Define.View viewMode;

    //// 쿼터뷰
    //// 


    //Vector3 offset;


    //[SerializeField]
    //Vector3 _quaterDelta = new Vector3(0, 12, -9);
    //[SerializeField]
    //Vector3 _thirdDelta = new Vector3(0, 2.7f, -5f);

    //// TODO
    ///*
    // * 1. ,카메라는 타겟(플레이어)가 필요
    // * 2. 플레이어 역할에 따라 시점을 달리 해야 함 강도: 쿼터뷰, 집주인: TPS
    // * 3. 강도에서 집주인으로 갈 때 위치가 모드가 전환되어야 함
    // */

    //void Start()
    //{
    //    _player = GameObject.Find("Player");
    //    _playerTransform = _player.transform;
        
    //    viewMode = Define.View.Third;

    //    // TODO
    //    /*
    //     * 1. 플레이어 찾기
    //     * 2. 적절한 모드 전환
    //     * 
    //     * */
    //}

    //void Update()
    //{
    //    if(Input.GetKeyUp(KeyCode.Q)) 
    //    {
    //        viewMode = Define.View.Quater;
    //        InitializeCameraTransform();
    //    }
    //    else if(Input.GetKeyUp(KeyCode.E))
    //    {
    //        viewMode = Define.View.Third;
    //        InitializeCameraTransform(); 
    //    }
    //    offset = transform.position - _playerTransform.position;
    //    TakeCamera();
    //}

    //void TakeCamera()
    //{
    //    if (viewMode == Define.View.Quater)
    //        QuaterView();
    //    else if (viewMode == Define.View.Third)
    //        ThirdView();
    //}

    //void QuaterView()
    //{
    //    transform.position = _player.transform.position + _quaterDelta;
    //    transform.rotation = new Quaternion(50f, 0f, 0f, 0f);       // 사선 
    //    transform.LookAt(_player.transform);
    //}

    //void ThirdView()
    //{
    //    LookAround();
    //    CameraMove();
    //    //TestMove();
    //    //Debug.DrawRay(_cameraArm.position, new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized, Color.red);
    //    Debug.DrawRay(transform.position, new Vector3(transform.forward.x, 0f, transform.forward.z).normalized, Color.red);
    //}

    ///// <summary>
    ///// 마우스에 따라 회전
    ///// </summary>
    //private void LookAround()
    //{
    //    Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    //    Vector3 camAngle = transform.rotation.eulerAngles;
    //    float x = camAngle.x - mouseDelta.y;

    //    // 회전 각도 지정
    //    if (x < 180f) // 위쪽으로 회전
    //        x = Mathf.Clamp(x, -1f, 70f);
    //    else // 아래쪽으로 회전
    //        x = Mathf.Clamp(x, 335f, 361f);

    //    transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    //}

    //private void CameraMove()
    //{
    //    float cameraPosZ = _playerTransform.position.z + _thirdDelta.z; // "_thirdDelta.z" is the initial value of _camerArm's position.z.
    //    Vector3 cameraPos = new Vector3(_playerTransform.position.x, transform.position.y, cameraPosZ);
    //    //_cameraArm.position = cameraPos;
    //    transform.position = cameraPos;
    //}

    //void InitializeCameraTransform()
    //{
    //    if(viewMode == Define.View.Quater)
    //    {
    //        transform.position = _playerTransform.position + _quaterDelta;
    //        transform.rotation = new Quaternion(50f, 0f, 0f, 0f);
    //    }
    //    else if(viewMode == Define.View.Third)
    //    {
    //        transform.position = _playerTransform.position + _thirdDelta;
    //        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    //    }
    //    transform.LookAt(_playerTransform);
    //}



    //// 캐릭터가 이동하는 방향을 기준으로 카메라 이동
    //private void TestMove()
    //{
        
    //    Debug.DrawRay(transform.position, new Vector3(transform.forward.x, 0f, transform.forward.z).normalized, Color.red);
    //}
}
