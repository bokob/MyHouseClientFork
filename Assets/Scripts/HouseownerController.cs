using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HouseownerController : PlayerController
{
    //bool isHoldGun = true;
    //bool isPressedAiming = false;

    //[SerializeField] float _rotationSpeed = 5f;
    
    //[Space(10)]
    
    //[Header ("무기 관련")]
    //// [SerializeField] protected Weapon weapon;
    //[SerializeField] protected WeaponManager weaponManager;
    //[SerializeField] public GameObject _leftItemHand;           // 왼손에 있는 아이템 (자식: 탄창)
    //[SerializeField] public GameObject _rightItemHand;          // 오른손에 있는 아이템 (자식: 무기)
    //[SerializeField] public GameObject _magazine;               // 탄창
    //[SerializeField] public GameObject _gun;                    // 총
    //[SerializeField] public Weapon _rifle;
    //[SerializeField] public GameObject _melee;                  // 근접 무기

    //private void Start()
    //{
    //    // 아이템 들 수 있는 손 찾기
    //    GameObject[] itemHand = GameObject.FindGameObjectsWithTag("Hand");
    //    Array.Sort(itemHand, (a, b) => a.name.CompareTo(b.name)); // 태그로 찾으면 순서 보장 X, 따라서 정렬

    //    _leftItemHand = itemHand[0];
    //     _rightItemHand = itemHand[1];

    //    // 무기 접근을 위한 변수 설정
    //    _magazine = _leftItemHand.transform.GetChild(0).gameObject;
    //    _gun = _rightItemHand.transform.GetChild(0).gameObject;
    //    _melee = _rightItemHand.transform.GetChild(1).gameObject;

    //    _rifle = _gun.GetComponent<Gun>();
    //}

    //void Update()
    //{
    //    // PlayerConroller의 dir로 이동을 입력받는다.
    //    base.MoveKeyInput();
    //    base.Jump();
    //    base.Dead();
    //}
    //void FixedUpdate()
    //{
    //    if(base._isDead) return;

    //    Walk();
    //    base.Run();

    //    // base.MeleeAttack();
    //    HoldGun();
    //    //_gun.Use();
    //    AimingGun();
    //    Reload();
    //}

    ///// <summary>
    ///// 집주인 전용 이동, TPS 시점이라 카메라가 보는 방향으로 이동하기 위해서
    ///// </summary>
    //protected override void Walk()
    //{
    //    Vector3 movementDirection = Quaternion.Euler(0, base._thirdCamera.transform.eulerAngles.y, 0) * new Vector3(base.dir.x, 0, base.dir.z);

    //    _moveSpeed = _walkSpeed;
    //    if (movementDirection != Vector3.zero)
    //    {
    //        Quaternion desiredRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationSpeed * Time.deltaTime);
    //        transform.position += movementDirection * _moveSpeed * Time.deltaTime;
    //    }
    //    anim.SetBool("isWalk", dir != Vector3.zero);
    //}


    //void HoldGun()
    //{
    //    isHoldGun = _gun.activeSelf;
    //    anim.SetBool("isHoldGun", isHoldGun);
    //}

    //protected void AimingGun()
    //{
    //    isPressedAiming = Input.GetMouseButton(1);
    //    anim.SetBool("isAim", isPressedAiming && isHoldGun);
    //}

    //#region 장전
    //void Reload()
    //{
    //    if(Input.GetKeyDown(KeyCode.R) && isHoldGun)
    //    {
    //        StopCoroutine("Reloading");
    //        anim.SetTrigger("setReload");
    //        StartCoroutine("Reloading");
    //    }
    //}

    //IEnumerator Reloading()
    //{
    //    _magazine.SetActive(true);
    //    yield return new WaitForSeconds(2f);
    //    _magazine.SetActive(false);
    //}
    //#endregion
}