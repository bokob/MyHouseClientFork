using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class P_ControllerMT : NetworkBehaviour
{
    public PlayerInputs _input;
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float SpeedChangeRate = 10.0f;
    public float RotationSmoothTime = 0.12f;
    protected CharacterController _controller;
#if ENABLE_INPUT_SYSTEM
    protected PlayerInput _playerInput;
#endif
    protected float _speed;
    protected float _targetRotation;
    protected Camera _mainCamera;
    protected float _verticalVelocity;
    protected float _rotationVelocity;
    protected bool _rotateOnMove = true;

    private void Awake() {
        _mainCamera = Camera.main;
        _mainCamera.cullingMask = ~(1 << 12);
    }
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
        _playerInput = GetComponent<PlayerInput>();
        SoundManager.Soundinstance.PlayBGM(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        Move();
        RoofTransP();

    }

    void RoofTransP()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            _mainCamera.cullingMask = ~1;
        }
    }
    void Move()
    {
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

        // 움직임 없으면 0 벡터로 처리
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            if (_rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // 플레이어 움직이게 하기
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    }
}
