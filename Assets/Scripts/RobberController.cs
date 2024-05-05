using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 강도 컨트롤러
/// </summary>
public class RobberController : PlayerController
{
    private void Update()
    {
        _hasAnimator = TryGetComponent(out base._animator);

        base.JumpAndGravity();
        base.GroundedCheck();
        base.Move();
        base.MeleeAttack();
    }

    private void LateUpdate()
    {
        //CameraRotation();
    }
}