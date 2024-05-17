#if UNITY_EDITOR
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

    private void Awake()
    {
        _target = GameObject.Find("ThirdCameraTarget").transform;
    }

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        _rotationY += mouseX;
        _rotationX += mouseY;

        // �� �Ʒ� ���� ����
        _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

        Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

        _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
        transform.localEulerAngles = _currentRotation;

        // ��ǥ �������� ī�޶��� ��ġ�� ���� ��ǥ������ �Ÿ��� ���ؼ� �׸�ŭ �������� ��ġ��Ų��.
        transform.position = _target.position - transform.forward * _distanceFromTarget;
    }
}
#endif