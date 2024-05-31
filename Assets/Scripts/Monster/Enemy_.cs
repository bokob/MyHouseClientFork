using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_ : MonoBehaviour
{
    NavMeshAgent nmAgent;
    Animator _anim;
    Status _status;

    // 순찰 관련
    public Transform centerPoint;  // 순찰 위치 정할 기준점
    public float range;            // 순찰 위치 정할 범위
    public float patrolSpeed = 1f; // 순찰 속도

    // 상태 관련
    public float Hp { get; private set; } = 300f;                   // 체력
    public Define.MonsterState _state = Define.MonsterState.Patrol; // 현재 상태
    public bool isDead = false;

    // 적 시야 관련
    public float radius;              // 시야 범위
    [Range(0, 360)]
    public float angle;               // 시야각
    public LayerMask targetMask;      // 목표
    public LayerMask obstructionMask; // 장애물
    public bool canSeePlayer;

    // 추격 관련
    public float chaseRange = 10f; // 추격 범위
    public float lostDistance; // 놓치는 거리

    // 공격 관련
    public float attackRange = 2f; // 공격 범위
    public float attackDelay = 2f; // 공격 간격
    float nextAttackTime = 0f;

    public Transform target = null; // 목표

    void Start()
    {
        Debug.Log("시작");
        _anim = GetComponent<Animator>();
        nmAgent = GetComponent<NavMeshAgent>();
        centerPoint = transform;
        _status = GetComponent<Status>();
        _status.Hp = Hp;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void Update()
    {
        if (isDead) return;

        FieldOfViewCheck(); // 시야에 플레이어 있는지 확인

        switch (_state)
        {
            case Define.MonsterState.Idle:
                StartCoroutine(Idle());
                break;
            case Define.MonsterState.Patrol:
                StartCoroutine(Patrol());
                break;
            case Define.MonsterState.Chase:
                StartCoroutine(Chase());
                break;
            case Define.MonsterState.Attack:
                StartCoroutine(Attack());
                break;
            case Define.MonsterState.Hit:
                break;
        }

    }

    void FieldOfViewCheck() // 시야
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform findTarget = rangeChecks[0].transform;
            Vector3 directionToTarget = (findTarget.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2) // 플레이어로부터 부채꼴처럼 볼 수 있게
            {
                float distanceToTarget = Vector3.Distance(transform.position, findTarget.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    target = findTarget; // 목표 설정
                    canSeePlayer = true; // 플레이어 감지
                }
                else // 벽 감지한 경우
                {
                    canSeePlayer = false;
                    target = null;
                }
            }
            else
            {
                canSeePlayer = false;
                target = null;
            }
        }
        else if (canSeePlayer) // 보고 있다가 시야에서 사라진거
        {
            canSeePlayer = false;
            target = null;
        }
    }

    IEnumerator Idle() // 대기
    {
        // 애니메이터 상태 정보 얻기
        AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (!currentAnimStateInfo.IsName("Idle"))
            _anim.Play("Idle", 0, 0);

        yield return new WaitForSeconds(currentAnimStateInfo.length);

        if (canSeePlayer) // 플레이어 관측
        {
            StopAllCoroutines();
            nmAgent.SetDestination(target.position); // 목표 지정
            ChangeState(Define.MonsterState.Chase);
        }
        else
        {
            StopAllCoroutines();
            ChangeState(Define.MonsterState.Patrol);
        }
    }
    IEnumerator Patrol() // 순찰
    {
        Debug.Log("순찰");
        // 애니메이터 상태 정보 얻기
        AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (!currentAnimStateInfo.IsName("Move"))
            _anim.Play("Move", 0, 0);

        // 랜덤하게 순찰 지점 정하기
        if (nmAgent.remainingDistance <= nmAgent.stoppingDistance) // 플레이어 못봤을 때
        {
            Vector3 point;
            if (RandomPoint(centerPoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 3.0f); // 갈 지점 표시

                nmAgent.SetDestination(point);

                yield return null;
            }
        }
        else if(canSeePlayer && nmAgent.remainingDistance <= nmAgent.stoppingDistance) // 공격 범위 안에 있을 때
        {
            StopAllCoroutines(); // 모든 코루틴 종료
            nmAgent.ResetPath();
            ChangeState(Define.MonsterState.Attack);  // 공격
        }
        else if(canSeePlayer && nmAgent.remainingDistance > nmAgent.stoppingDistance) // 공격범위 밖이면 추격
        {
            StopAllCoroutines(); // 모든 코루틴 종료
            nmAgent.SetDestination(target.position); // 목표 지정
            ChangeState(Define.MonsterState.Chase);  // 추격
        }
    }

    IEnumerator Chase() // 추격
    {
        AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (!currentAnimStateInfo.IsName("Move"))
        {
            _anim.Play("Move", 0, 0);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return null;
        }

        // 목표까지의 남은 거리가 멈추는 지점보다 작거나 같으면
        if (canSeePlayer && nmAgent.remainingDistance <= nmAgent.stoppingDistance)
        {
            StopAllCoroutines();
            nmAgent.ResetPath();
            ChangeState(Define.MonsterState.Attack);
        }
        else if(canSeePlayer) // 목표가 시야에 있는데 계속 움직이면 경로 다시 계산해서 추격
        {
            nmAgent.SetDestination(target.position);
        }
        else if (!canSeePlayer) // 시야에서 사라졌으면 Idle로 전환
        {
            StopAllCoroutines();
            nmAgent.ResetPath();
            ChangeState(Define.MonsterState.Idle);
            yield return null;
        }
        else
        {
            // 애니메이션의 한 사이클 동안 대기
            yield return new WaitForSeconds(currentAnimStateInfo.length);
        }
    }

    IEnumerator Attack()
    {
        AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        nmAgent.isStopped = true;

        if (target==null) // 목표물이 사라지면
        {
            StopAllCoroutines();
            nmAgent.isStopped = false;
            ChangeState(Define.MonsterState.Patrol); // 순찰
        }
        else nmAgent.SetDestination(target.position);

        if (!currentAnimStateInfo.IsName("Attack"))
        {
            _anim.Play("Attack", 0, 0);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return null;
        }

        // 시야 범위에서 사라지면
        if (!canSeePlayer)
        {
            StopAllCoroutines();
            nmAgent.isStopped = false;
            ChangeState(Define.MonsterState.Patrol); // 순찰
        }
        else if(canSeePlayer && nmAgent.remainingDistance > nmAgent.stoppingDistance)
        {
            nmAgent.isStopped = false;
            ChangeState(Define.MonsterState.Chase);
        }

        yield return null;
    }

    IEnumerator OnHit(Collider other) {
        if (_state != Define.MonsterState.None)
        {
            AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

            if (!currentAnimStateInfo.IsName("Surprised"))
            {
                _anim.Play("Surprised", 0, 0);
                currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);
                // SetDestination 을 위해 한 frame을 넘기기위한 코드
                yield return new WaitForSeconds(currentAnimStateInfo.length);
            }

            // 데미지 적용
            _status.TakedDamage(other.GetComponent<Weapon>().Attack);
            Debug.Log($"{gameObject.name}이(가) {other.transform.root.name}에게 공격 받음!");
            Debug.Log("공격받은 사람의 체력:" + _status.Hp);

            if (_status.Hp <= 0)
                Dead();
            else
                ChangeState(Define.MonsterState.Attack);
        }
    }

    void ChangeState(Define.MonsterState newState)
    {
        _state = newState;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_state == Define.MonsterState.None) return;

        // 태그가 무기 태그인 경우
        if (other.tag == "Melee" || other.tag == "Gun")
        {
            if (Hp > 0)
            {
                StopAllCoroutines();
                ChangeState(Define.MonsterState.Hit);
                StartCoroutine(OnHit(other));
            }
        }
        else 
            Debug.Log("왜 안닿지?");
    }

    public void Dead()
    {
        if (_state != Define.MonsterState.None && _status.Hp <= 0)
        {
            isDead = true;
            nmAgent.ResetPath();
            _anim.Play("Die", 0, 0);
            _state = Define.MonsterState.None; // 시체
            StartCoroutine(DeadSinkCoroutine());
        }
    }

    IEnumerator DeadSinkCoroutine()
    {
        Debug.Log("시체처리");
        nmAgent.enabled = false; // 안하면 밑으로 안내려가짐
        yield return new WaitForSeconds(3f);
        while (transform.position.y > -1.5f)
        {
            Debug.Log("땅속으로 들어가는중");
            transform.Translate(Vector3.down * 0.1f * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}