 using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;
#endif

// 진짜 집주인 컨트롤러
public class HouseownerController : PlayerController
{
    private void Update()
    {
        base._hasAnimator = TryGetComponent(out base._animator);

        base.JumpAndGravity();
        base.GroundedCheck();
        base.Move();

        if (base._melee == null)
        {
            base.PlayerWeaponInit();

            if (base._melee != null) Debug.Log("근접 무기 인식!");
            if (base._gun != null) Debug.Log("총기 인식!");
        }

        if (base._melee.activeSelf)
            base.MeleeAttack();
        else if (base._gun.activeSelf)
            base.gunWeapon.Use();

        // 총 관련해서 행동 안하고 있을 때만 무기 바꾸기
        if (!base._input.aim && !base._input.reload)
            base.weaponManager.HandleWeaponSwitching();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void Init()
    {

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

    public void ChangeIsHoldGun(bool newIsHoldGun)
    {
        base._animator.SetBool("isHoldGun", newIsHoldGun);
    }
}
