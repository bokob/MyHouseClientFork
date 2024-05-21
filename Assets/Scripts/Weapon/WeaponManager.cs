using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Tooltip("무기 전환 시 지연 시간을 설정")]
    public float switchDelay = 1f;

    [Tooltip("플레이어가 사용할 수 있는 무기 목록")]
    public List<GameObject> weaponList = new List<GameObject>();

    private int index = 0;
    private bool isSwitching = false;

    private PlayerController playerController;
    private RobberController robberController;
    private HouseownerController houseownerController;


    [Header("무기 관련")]
    [SerializeField] public GameObject _leftItemHand;           // 왼손에 있는 아이템 (자식: 탄창)
    [SerializeField] public GameObject _rightItemHand;          // 오른손에 있는 아이템 (자식: 무기)
    [SerializeField] public GameObject _melee;                  // 근접 무기 오브젝트
    [SerializeField] public GameObject _gun;
    public Melee meleeWeapon; // 근접 무기
    public Gun gunWeapon;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        robberController = transform.GetChild(0).GetComponent<RobberController>();
        houseownerController = transform.GetChild(1).GetComponent<HouseownerController>();
    }

    public void PlayerWeaponInit() // 무기 세팅
    {
        // 무기 리스트 비우기
        ClearWeaponList();

        // 무기 찾기
        // 하위의 모든 자식 오브젝트 중 "Hand" 태그를 갖는 활성화된 오브젝트만 찾기
        GameObject[] itemHand = GameObject.FindGameObjectsWithTag("Hand");

        // 이름순으로 정렬
        Array.Sort(itemHand, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

        if (itemHand.Length < 2)
        {
            Debug.LogError("Both left and right hand objects are not found.");
            return;
        }

        _leftItemHand = itemHand[0];
        _rightItemHand = itemHand[1];

        // 근접 무기 접근을 위한 변수 설정
        _melee = _rightItemHand.transform.GetChild(0).gameObject;

        meleeWeapon = _melee.GetComponent<Melee>();
        AddWeaponInList(_melee);

        // 원거리 무기 접근을 위한 변수 설정
        if (_rightItemHand.transform.childCount == 2) // 오른손에 무언가 있는 경우
        {
            _gun = _rightItemHand.transform.GetChild(1).gameObject;
            gunWeapon = _gun.GetComponent<Gun>();
            AddWeaponInList(_gun);
        }
    }



    // 무기 전환 입력을 처리하는 메서드
    public void HandleWeaponSwitching()
    {
        if (!isSwitching)
        {
            // 마우스 휠 위로 스크롤 시 다음 무기로 전환
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                index = (index + 1) % weaponList.Count;
                StartCoroutine(SwitchDelay(index));
            }
            // 마우스 휠 아래로 스크롤 시 이전 무기로 전환
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                index = (index - 1 + weaponList.Count) % weaponList.Count;
                StartCoroutine(SwitchDelay(index));
            }

            // 숫자 키(1~9)로 무기 전환
            for (int i = 49; i < 58; i++)
            {
                int keyIndex = i - 49;
                if (Input.GetKeyDown((KeyCode)i) && weaponList.Count > keyIndex && index != keyIndex)
                {
                    index = keyIndex;
                    StartCoroutine(SwitchDelay(index));
                }
            }
        }
    }

    // 무기 목록에 새로운 무기를 추가
    public void AddWeaponInList(GameObject weaponObject)
    {
        if (weaponObject != null && !weaponList.Contains(weaponObject))
            weaponList.Add(weaponObject);
    }

    // 무기 목록에서 특정 무기를 제거
    public void RemoveWeaponInList(GameObject weaponObject)
    {
        if (weaponObject != null && weaponList.Contains(weaponObject))
            weaponList.Remove(weaponObject);
    }

    public void ClearWeaponList()
    {
        weaponList.Clear();
    }

    // 무기를 초기 상태로 설정
    public void InitializeWeapon()
    {
        ClearWeaponList();
        PlayerWeaponInit();

        // 모든 무기를 비활성화
        foreach (GameObject weapon in weaponList)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }

        // 역할에 따른 첫 무기 설정
        if(playerController.PlayerRole == Define.Role.Houseowner) // 집주인
        {
            //houseownerController.ChangeIsHoldGun(true);
            index = Mathf.Clamp(1, 0, weaponList.Count - 1); // 인덱스가 리스트 범위 내에 있는지 확인
            if (weaponList.Count > index)
                weaponList[index].SetActive(true);
        }
        else if(playerController.PlayerRole == Define.Role.Robber) // 강도
        {
            index = Mathf.Clamp(0, 0, weaponList.Count - 1);
            if (weaponList.Count > index)
                weaponList[index].SetActive(true);
        }

        Debug.Log("무사히 무기 세팅 완료");
    }

    // 무기 전환 시 지연 시간을 추가하여 무한 전환 방지
    private IEnumerator SwitchDelay(int newIndex)
    {
        isSwitching = true;
        SwitchWeapons(newIndex);
        yield return new WaitForSeconds(switchDelay);
        isSwitching = false;
    }

    private void SwitchWeapons(int newIndex)
    {
        // 모든 무기를 비활성화
        for (int i = 0; i < weaponList.Count; i++)
        {
            if (weaponList[i] != null)
                weaponList[i].SetActive(false);
        }

        // 무기 인덱스가 유효한지 확인
        if (newIndex < 0 || newIndex >= weaponList.Count || weaponList[newIndex] == null)
        {
            return;
        }

        // 근접 무기인지 여부에 따라 집주인의 무기 상태를 변경
        if (weaponList[newIndex].CompareTag("Melee"))
        {
            if (houseownerController != null)
                playerController.ChangeIsHoldGun(false);
        }
        else
        {
            if (houseownerController != null)
                playerController.ChangeIsHoldGun(true);
        }

        // 새로운 무기 활성화
        weaponList[newIndex].SetActive(true);
    }
}