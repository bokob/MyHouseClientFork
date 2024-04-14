using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _cameraArm;

    void Update()
    {
        LookAround();
        CameraMove();
        //TestMove();
    }


    private void TestMove()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        if(isMove)
        {
            Vector3 lookForard = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
            Vector3 moveDir = lookForard * moveInput.y + lookRight * moveInput.x;

            //playerbody.forward = moveDir;
            //transform.position += moveDir * Time.deltaTime * 5f;
        }

        Debug.DrawRay(_cameraArm.position, new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized, Color.red);
    }

    /// <summary>
    /// 마우스에 따라 회전
    /// </summary>
    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 70f);
        else
            x = Mathf.Clamp(x, 335f, 361f);

        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void CameraMove()
    {
        float cameraPosZ = -0.5f + _target.position.z; // -0.5f is the initial value of _camerArm's position.z.
        Vector3 cameraPos = new Vector3(_target.position.x, _cameraArm.position.y, cameraPosZ);
        _cameraArm.position = cameraPos;
    }
}
