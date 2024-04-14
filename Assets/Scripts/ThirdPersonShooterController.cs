using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    private Transform hitTransform;
    [SerializeField] private GameObject vfxHitGreen;
    [SerializeField] private GameObject vfxHitRed;

    private TestHouseownerController houseownerController;
    private PlayerInputs playerInputs;
    private Animator animator;

    Vector3 mouseWorldPosition;

    private void Awake()
    {
        houseownerController = GetComponent<TestHouseownerController>();
        playerInputs = GetComponent<PlayerInputs>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HitRayCheck();
        Aim();
        Shoot();
    }

    void HitRayCheck()
    {
        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * raycastHit.distance, Color.red);
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }
        else // 충돌 안했을 때
        {
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
        }
    }

    void Aim()
    {
        // 정조준
        if (playerInputs.aim)
        {
            // 조준 시점으로 카메라 변경
            aimVirtualCamera.gameObject.SetActive(true);
            houseownerController.SetSensitivity(aimSensitivity);
            houseownerController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            animator.SetBool("isAim", true);
            animator.SetTrigger("setAim");

            // 조준하는 방향으로 회전
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            animator.SetBool("isAim", false);
            // 원래 시점으로 카메라 변경
            aimVirtualCamera.gameObject.SetActive(false);
            houseownerController.SetSensitivity(normalSensitivity);
            houseownerController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

        }
    }

    void Shoot()
    {
        // 발사
        if (playerInputs.shoot)
        {
            Debug.Log("발사");
            if (hitTransform != null)
            {
                Debug.Log("너 걸렸다");
                // 무언가 맞았으면
                if (hitTransform.GetComponent<BulletTarget>() != null)
                {
                    GameObject Effect = Instantiate(vfxHitGreen, mouseWorldPosition, Quaternion.identity);
                    Destroy(Effect, 0.5f);
                }
                else
                {
                    Instantiate(vfxHitRed, mouseWorldPosition, Quaternion.identity);
                }
            }
            //Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            //Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            playerInputs.shoot = false;
        }
    }
    void HitEffect(RaycastHit hit) // 피격효과
    {
        GameObject Effect = Instantiate(vfxHitGreen, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(Effect, 0.5f);
    }
}
