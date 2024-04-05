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
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if(x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void CameraMove()
    {
        float cameraPosZ = -0.5f + _target.position.z; // -0.5f is the initial value of _camerArm's position.z.
        Vector3 cameraPos = new Vector3(_target.position.x, _cameraArm.position.y, cameraPosZ);
        _cameraArm.position = cameraPos;
    }
}
