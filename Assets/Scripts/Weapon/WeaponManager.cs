using System.Collections;
using System.Collections.Generic;
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
        playerController = GetComponent<PlayerController>();
        houseownerController = GetComponent<HouseownerController>();
        robberController = GetComponent<RobberController>();

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
                houseownerController.ChangeIsHoldGun(false);
        }
        else
        {
            if (houseownerController != null)
                houseownerController.ChangeIsHoldGun(true);
        }

        // 새로운 무기 활성화
        weaponList[newIndex].SetActive(true);
    }
}