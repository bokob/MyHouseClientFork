using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 강도 컨트롤러
/// </summary>
public class RobberController : PlayerController
{
    GameObject quaterFollowCamera;
    private void Update()
    {
        _hasAnimator = TryGetComponent(out base._animator);

        base.JumpAndGravity();
        base.GroundedCheck();
        base.Move();
        base.MeleeAttack();

        if (Input.GetKeyDown(KeyCode.T))
            base.ChangeToHouseowner();
    }

    private void LateUpdate()
    {

    }
}