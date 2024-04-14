 using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

public class TestHouseownerController : PlayerController
{
    
    private void Update()
    {
        base._hasAnimator = TryGetComponent(out base._animator);

        base.JumpAndGravity();
        base.GroundedCheck();
        base.Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }


    // 카메라 각도 제한
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    // 카메라 회전
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            // 정조준 할 때 천천히 돌아가야 하니까 Sensitivity를 넣어준다.
            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * Sensitivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * Sensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // 시네마신 카메라가 목표를 따라감
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    public void SetSensitivity(float newSensitivity)
    {
        Sensitivity = newSensitivity;
    }

    public void SetRotateOnMove(bool newRotateOnMove)
    {
        _rotateOnMove = newRotateOnMove;
    }
}
