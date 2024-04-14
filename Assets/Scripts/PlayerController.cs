using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어(집주인, 강도)가 공통적으로 상속받는 클래스
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    // 움직임 속도
    public float MoveSpeed = 2.0f;
    // 달리기 속도
    public float SprintSpeed = 5.335f;

    [Range(0.0f, 0.3f)]
    // 움직임 방향 전환
    public float RotationSmoothTime = 0.12f;

    // 속도 가속
    public float SpeedChangeRate = 10.0f;

    public float Sensitivity = 1f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    // 점프 높이
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    // 점프 쿨타임
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    // 지면에 닿았는지 여부
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    protected float _cinemachineTargetYaw;
    protected float _cinemachineTargetPitch;

    // player
    protected float _speed;
    protected float _animationBlend;
    protected float _targetRotation = 0.0f;
    protected float _rotationVelocity;
    protected float _verticalVelocity;
    protected float _terminalVelocity = 53.0f;

    // timeout deltatime
    protected float _jumpTimeoutDelta;
    protected float _fallTimeoutDelta;

    // animation IDs
    protected int _animIDSpeed;
    protected int _animIDGrounded;
    protected int _animIDJump;
    protected int _animIDFreeFall;
    protected int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
    protected PlayerInput _playerInput;
#endif
    protected Animator _animator;
    protected CharacterController _controller;
    protected PlayerInputs _input;
    protected GameObject _mainCamera;
    protected bool _rotateOnMove = true;

    protected const float _threshold = 0.01f;

    protected bool _hasAnimator;

    protected bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
			return false;
#endif
        }
    }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
#if ENABLE_INPUT_SYSTEM 
        _playerInput = GetComponent<PlayerInput>();
#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    // 애니메이션 파라미터 해시로 관리
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    // 이동
    protected void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
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

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

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

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    // 지면 체크
    protected void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        // "Grounded" 애니메이션 파라미터 변경
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }


    // 점프
    protected void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    // 땅에 닿을 때 착지 소리 나게 하는 애니메이션 이벤트
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }


    //#region 구버전
    //protected string _nickname;  // 닉네임
    //protected Define.Role _role; // 플레이어 역할

    //[Header("컴포넌트")]
    //protected Rigidbody rb;
    //protected Animator anim;
    //protected Camera _thirdCamera;

    //[Space(10)]

    //[Header ("이동 관련")]
    //protected Vector3 dir = Vector3.zero;
    //protected bool _isGround;
    //protected float _moveSpeed;
    //protected float _walkSpeed = 5f;
    //protected float _runSpeed = 15f;
    //protected float _jumpHeight = 4f;         // 점프 파워
    //bool isPressedRunKey;                     // 달리는 상태 판별

    //[Space(10)]

    //[Header ("상태 관련")]
    //protected bool _isDead;                    // 죽었는지 판별
    //protected Status _status;

    //[Space(10)]

    //[Header ("공격 관련")]
    //bool swingKeyDown;  // 마우스 왼쪽 키 눌렸는지
    //bool isSwingReady;  // 공격 준비
    //float swingDelay;   // 공격 딜레이
    //bool stabKeyDown;  // 마우스 왼쪽 키 눌렸는지
    //bool isStabReady;  // 공격 준비
    //float stabDelay;   // 공격 딜레이
    //GameObject _rightHand;
    //public Melee meleeWeapon; // 근접 무기

    //protected void Awake()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    anim = GetComponentInChildren<Animator>();

    //    //_thirdCamera = GameObject.Find("ThirdCamera").GetComponent<Camera>();

    //    gameObject.AddComponent<Status>();
    //    _status = gameObject.GetComponent<Status>();
    //}

    //protected void MoveKeyInput()
    //{
    //    dir.x = Input.GetAxis("Horizontal");
    //    dir.z = Input.GetAxis("Vertical");
    //    dir = dir.normalized;
    //}

    ///// <summary>
    ///// 기본 이동, 걷는 애니메이션 재생
    ///// </summary>
    //protected virtual void Walk()
    //{
    //    _moveSpeed = _walkSpeed;
    //    if (dir != Vector3.zero)
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
    //        transform.position += dir * _moveSpeed * Time.deltaTime;
    //    }
    //    anim.SetBool("isWalk", dir != Vector3.zero);
    //}

    ///// <summary>
    ///// 달리기, 이동 속도를 변화시키고 달리는 애니메이션 재생
    ///// </summary>
    //protected virtual void Run() // 달리는 속도로 만들기
    //{
    //    isPressedRunKey = Input.GetKey(KeyCode.LeftShift);
    //    if (isPressedRunKey)
    //    {
    //        _moveSpeed = _runSpeed;
    //    }
    //    else
    //        _moveSpeed = _walkSpeed;
    //    anim.SetBool("isRun", isPressedRunKey && dir != Vector3.zero);
    //}

    ///// <summary>
    ///// Ground인지 판단
    ///// </summary>
    //protected void IsGround()
    //{
    //    Debug.DrawRay(transform.position + (Vector3.up * 0.2f), Vector3.down, Color.red);

    //    RaycastHit hit;
    //    int layerMask = 1 << LayerMask.NameToLayer("Ground");
    //    if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.3f, layerMask))
    //        _isGround = true;
    //    else
    //        _isGround = false;
    //}

    ///// <summary>
    ///// 점프
    ///// </summary>
    //protected void Jump()
    //{
    //    IsGround();
    //    if (Input.GetKeyDown(KeyCode.Space) && _isGround)
    //    {
    //        Vector3 jumpPower = Vector3.up * _jumpHeight;
    //        rb.AddForce(jumpPower, ForceMode.VelocityChange);
    //        anim.SetTrigger("setJump");
    //    }
    //}

    ///// <summary>
    ///// hp가 0이되면 사망
    ///// </summary>
    //protected void Dead()
    //{
    //    if (_status.Hp <= 0 || Input.GetKeyDown(KeyCode.P))
    //    {
    //        anim.SetTrigger("setDie");
    //        _isDead = true;
    //    }
    //}


    ///// <summary>
    ///// 강도의 근접 공격: 좌클릭(휘두르기), 우클릭(찌르기)
    ///// </summary>
    //protected void MeleeAttack()
    //{
    //    swingKeyDown = Input.GetMouseButtonDown(0);
    //    stabKeyDown = Input.GetMouseButtonDown(1);

    //    if (meleeWeapon == null)
    //    {
    //        Debug.Log("현재 장착된 무기가 없음");
    //        return;
    //    }

    //    swingDelay += Time.deltaTime;
    //    stabDelay += Time.deltaTime;
    //    isSwingReady = meleeWeapon.Rate < swingDelay; // 공격속도가 공격 딜레이보다 작으면 공격준비 완료
    //    isStabReady = meleeWeapon.Rate < stabDelay;

    //    if (swingKeyDown && isSwingReady && _isGround) // 휘두르기
    //    {
    //        Debug.Log("시작");
    //        meleeWeapon.Use();
    //        anim.SetTrigger("setSwing");
    //        swingDelay = 0;
    //        swingKeyDown = false;
    //    }
    //    else if (stabKeyDown && isStabReady && _isGround) // 찌르기
    //    {
    //        Debug.Log("시작");
    //        meleeWeapon.Use();
    //        anim.SetTrigger("setStab");
    //        stabDelay = 0;
    //        stabKeyDown = false;
    //    }
    //}
    //#endregion

}