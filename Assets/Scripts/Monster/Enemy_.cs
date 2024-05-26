using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR;

public class Enemy_ : MonoBehaviour
{
    public Transform target; 

    public float patrolSpeed = 3f; // 순찰 속도
    public float returnToOriginalDistance = 1f; // 원래 위치로 돌아갈 때의 거리
    public float chaseRange = 10f; // 추격 범위
    public float attackRange = 2f; // 공격 범위
    public float attackDelay = 2f; // 공격 간격

    private NavMeshAgent nmAgent;
    private Animator _anim;
    public Transform centerPoint; // 맵의 중앙
    private float nextAttackTime = 0f;

    public float lostDistance;

    public float range;

    public float Hp { get; private set; } = 100f; // 체력

    public Define.MonsterState _state = Define.MonsterState.Idle;

    public FieldOfView fieldOfView;

    void Start()
    {
        Debug.Log("시작");
        _anim = GetComponent<Animator>();
        nmAgent = GetComponent<NavMeshAgent>();
        centerPoint = transform;
        fieldOfView = GetComponent<FieldOfView>();


    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
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

        switch(_state)
        {
            case Define.MonsterState.Idle:
                StartCoroutine(Idle());
                break;
            case Define.MonsterState.Patrol:
                StartCoroutine(Patrol());
                break;
        }

    }

    IEnumerator Idle() // 대기
    {
        // 애니메이터 상태 정보 얻기
        AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (!currentAnimStateInfo.IsName("Idle"))
            _anim.Play("Idle", 0, 0);

        yield return new WaitForSeconds(currentAnimStateInfo.length);

        //float distance = Vector3.Distance(transform.position, target.transform.position);
        //if (distance < )
        
        if(fieldOfView.canSeePlayer)
            ChangeState(Define.MonsterState.Chase);
        else
            ChangeState(Define.MonsterState.Patrol);
    }
    IEnumerator Patrol() // 순찰
    {

        Debug.Log("순찰");
        // 애니메이터 상태 정보 얻기
        AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (!currentAnimStateInfo.IsName("Move"))
            _anim.Play("Move", 0, 0);

        // 랜덤하게 순찰 지점 정하기
        if (nmAgent.remainingDistance <= nmAgent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centerPoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 3.0f);
                Debug.Log(point);
                nmAgent.SetDestination(point);

                yield return null;
            }
        }

        if(nmAgent.remainingDistance <= nmAgent.stoppingDistance)
            ChangeState(Define.MonsterState.Idle);
    }

    IEnumerator Chase() // 추격
    {
        AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (!currentAnimStateInfo.IsName("Move"))
        {
            _anim.Play("WalkFWD", 0, 0);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return null;
        }

        // 목표까지의 남은 거리가 멈추는 지점보다 작거나 같으면
        if (nmAgent.remainingDistance <= nmAgent.stoppingDistance)
        {
            // StateMachine 을 공격으로 변경
            ChangeState(Define.MonsterState.Attack);
        }
        // 목표와의 거리가 멀어진 경우
        else if (nmAgent.remainingDistance > lostDistance)
        {
            target = null;
            nmAgent.SetDestination(transform.position);
            yield return null;
            // StateMachine 을 대기로 변경
            ChangeState(Define.MonsterState.Idle);
        }
        else
        {
            // 애니메이션의 한 사이클 동안 대기
            yield return new WaitForSeconds(currentAnimStateInfo.length);
        }
    }

    void CheckPlayerAround()
    {

    }

    //IEnumerator ATTACK()
    //{
    //    AnimatorStateInfo currentAnimStateInfo = _anim.GetCurrentAnimatorStateInfo(0);

    //    // 공격 애니메이션은 공격 후 Idle Battle 로 이동하기 때문에 
    //    // 코드가 이 지점에 오면 무조건 Attack01 을 Play
    //    _anim.Play("Attack01", 0, 0);

    //    // 거리가 멀어지면
    //    if (nmAgent.remainingDistance > nmAgent.stoppingDistance)
    //    {
    //        // StateMachine을 추적으로 변경
    //        ChangeState(Define.MonsterState.Chase);
    //    }
    //    else
    //        // 공격 animation 의 두 배만큼 대기
    //        // 이 대기 시간을 이용해 공격 간격을 조절할 수 있음.
    //        yield return new WaitForSeconds(currentAnimStateInfo.length * 2f);
    //}

    //IEnumerator KILLED()
    //{
    //    yield return null;
    //}

    void ChangeState(Define.MonsterState newState)
    {
        _state = newState;
    }

    //IEnumerator StateMachine()
    //{
    //    while(Hp > 0)
    //    {
    //        yield return StartCoroutine(_state.ToString());
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.name != "Player") return;
    //    // Sphere Collider 가 Player 를 감지하면      
    //    target = other.transform;
    //    // NavMeshAgent의 목표를 Player 로 설정
    //    nmAgent.SetDestination(target.position);
    //    // StateMachine을 추적으로 변경
    //    ChangeState(Define.MonsterState.Chase);
    //}
}