using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 강도 컨트롤러
/// </summary>
public class RobberController : MonoBehaviour
{
    PlayerController playerController;
    WeaponManager weaponManager;

    [Tooltip("집주인으로 변할 때 사용")]
    RuntimeAnimatorController houseownerAnimController;
    Avatar houseownerAvatar;

    [Tooltip("카메라")]
    GameObject cameras;
    GameObject quaterFollowCamera;
    GameObject thirdFollowCamera;
    GameObject aimCamera;

    void Awake()
    {
    }

    private void Start()
    {
        // 강도 세팅
        RobberInit();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.T)) // 'T' 누르면 집주인으로 변신
            TransformationHouseowner();
    }

    void RobberInit()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
        weaponManager = transform.parent.GetComponent<WeaponManager>();

        playerController.PlayerRole = Define.Role.Robber;

        PrepareToBeHouseowner();
        CameraInit();
        RobberWeaponInit();
    }

    void PrepareToBeHouseowner()
    {
        // 강도->집주인 시에 사용할 집주인 애니메이션 관련된 것들 준비
        houseownerAnimController = Resources.Load<RuntimeAnimatorController>("Animations/HouseownerAnimations/HouseownerAnimationController");
        houseownerAvatar = Resources.Load<Avatar>("Animations/HouseownerAnimations/HouseownerAvatar");
    }

    void CameraInit() // 카메라 세팅
    {
        // 카메라 오브젝트 세팅
        cameras = Camera.main.gameObject.transform.parent.gameObject;
        quaterFollowCamera = cameras.transform.GetChild(1).gameObject;
        thirdFollowCamera = cameras.transform.GetChild(2).gameObject;
        aimCamera = cameras.transform.GetChild(3).gameObject;

        // 강도에 맞는 카메라 설정
        quaterFollowCamera.SetActive(true);
        thirdFollowCamera.SetActive(false);
        aimCamera.SetActive(false);
    }

    void RobberWeaponInit() // 강도 무기 세팅
    {
        weaponManager.InitializeWeapon();
    }

    void TransformationHouseowner()
    {
        transform.parent.GetChild(0).gameObject.SetActive(false); // 강도 비활성화
        transform.parent.GetChild(1).gameObject.SetActive(true);  // 집주인 활성화
        playerController.PlayerRole = Define.Role.Houseowner;

        Debug.Log(playerController.PlayerRole);

        weaponManager.InitializeWeapon();                         // 집주인 무기 세팅
        Debug.Log("집주인으로 변신 완료");
    }
}