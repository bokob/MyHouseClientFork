using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;
#endif

// 진짜 집주인 컨트롤러
public class HouseownerController : MonoBehaviour
{
    PlayerController playerController;
    WeaponManager weaponManager;

    [Tooltip("카메라")]
    GameObject cameras;
    GameObject quaterFollowCamera;
    GameObject thirdFollowCamera;
    GameObject aimCamera;

    private void Awake()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
        weaponManager = transform.parent.GetComponent<WeaponManager>();
    }

    private void Start()
    {
        HouseownerInit();
    }
    private void Update()
    {
        // 총 관련해서 행동 안하고 있을 때만 무기 바꾸기
        if (!playerController._input.aim && !playerController._input.reload)
            weaponManager.HandleWeaponSwitching();

        if (weaponManager._melee.activeSelf) // 근접 무기가 있는 경우에만 공격
            playerController.MeleeAttack();
        else if (weaponManager._gun.activeSelf)
            weaponManager.gunWeapon.Use();

        // 총 관련해서 행동 안하고 있을 때만 무기 바꾸기
        if (!playerController._input.aim && !playerController._input.reload)
            weaponManager.HandleWeaponSwitching();
    }

    private void HouseownerInit()
    {
        playerController.PlayerRole = Define.Role.Houseowner;

        RuntimeAnimatorController houseAnimController = gameObject.GetComponent<Animator>().runtimeAnimatorController;
        Avatar houseAvatar = gameObject.GetComponent<Animator>().avatar;
        playerController.SetRoleAnimator(houseAnimController, houseAvatar);

        CameraInit();
    }

    void CameraInit() // 카메라 세팅
    {
        // 카메라 오브젝트 세팅
        cameras = Camera.main.gameObject.transform.parent.gameObject;
        quaterFollowCamera = cameras.transform.GetChild(1).gameObject;
        thirdFollowCamera = cameras.transform.GetChild(2).gameObject;
        aimCamera = cameras.transform.GetChild(3).gameObject;

        // 집주인에 맞는 카메라 설정
        quaterFollowCamera.SetActive(false);
        thirdFollowCamera.SetActive(true);
        aimCamera.SetActive(true);
    }

    public void ChangeIsHoldGun(bool newIsHoldGun)
    {
        //base._animator.SetBool("isHoldGun", newIsHoldGun);
    }
}
