using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

//TODO
// Player에서 접근하는게 아닌 총 자체에서 사용하는 것으로 하기
public class Gun : Weapon
{
    #region 총 관련 변수
    [Tooltip("사거리")] [SerializeField] float range;
    [Tooltip("재장전 시간")] [SerializeField] float reloadTime;
    [Tooltip("한 번 장전 시, 장전할 탄약 수")] [SerializeField] int reloadBulletCount;
    [Tooltip("현재 탄약 수")] [SerializeField] int currentBulletCount;
    [Tooltip("최대 탄약 수")] [SerializeField] int maxBulletMagazine;
    [Tooltip("총 탄약 수")] [SerializeField] int totalBulletCount;
    [Tooltip("총 소리")] [SerializeField] AudioClip fireSound;
    [Tooltip("음원")] [SerializeField] AudioSource _audioSource;
    [Tooltip("총구 섬광 효과")] [SerializeField] ParticleSystem muzzleFlash;
    [Tooltip("연사 속도")] [SerializeField] float fireRate = 15f;
    [Tooltip("다음 격발 타이밍")] [SerializeField] float nextTimeToFire = 0f;
    #endregion

    #region 사격 및 조준 관련 변수
    [Tooltip("조준 카메라")] [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    [Tooltip("일반 마우스 민감도")] [SerializeField] float normalSensitivity;
    [Tooltip("조준 마우스 민감도")] [SerializeField] float aimSensitivity;
    [Tooltip("조준 가능 Layer")] [SerializeField] LayerMask aimColliderLayerMask;
    [Tooltip("조준하고 있는 위치")] [SerializeField] Transform debugTransform;
    [Tooltip("발사되는 총알")] [SerializeField] Transform pfBulletProjectile;
    [Tooltip("총알 발사되는 위치")] [SerializeField] Transform spawnBulletPosition;
    [Tooltip("Raycast 맞은 오브젝트")] [SerializeField] Transform hitTransform;
    [Tooltip("피격 O 여부")] [SerializeField] GameObject vfxHitGreen;
    [Tooltip("피격 X 오브젝트")] [SerializeField] GameObject vfxHitRed;
    [Tooltip("재장전 중인지 여부")] [SerializeField] bool isReload = false;
    [Tooltip("사격 중인지 여부")] [SerializeField] bool isShoot = false;
    [Tooltip("조준 중인지 여부")] [SerializeField] bool isAim = false;
    [Tooltip("마우스 조준 좌표")][SerializeField] Vector3 mouseWorldPosition;

    public BloodEffect blood;

    PlayerController playerController;
    PlayerInputs playerInputs;
    Animator animator;
    public RigBuilder rigBuilder; // IK 활성/비활성화를 조절하기 위해 접근
    #endregion

    Vector3 originalRotation; // 총의 원래 회전값

    void Start()
    {
        originalRotation = transform.localEulerAngles;

        playerController = base.Master.gameObject.GetComponent<PlayerController>();
        playerInputs = base.Master.gameObject.GetComponent<PlayerInputs>();
        animator = base.Master.gameObject.GetComponent<Animator>();
        //rigBuilder = transform.root.GetChild(0).GetComponent<RigBuilder>();
    }

    void HitRayCheck()
    {
        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (isShoot) // 쏠 때 반동
        {
            Debug.Log("<반동>");
            animator.Play("Recoil");
            float recoilAmount = 20f; // 반동 정도

            // 반동을 위한 무작위한 변위 생성
            float randomX = Random.Range(-recoilAmount, recoilAmount);
            float randomY = Random.Range(-recoilAmount, recoilAmount);

            screenCenterPoint += new Vector2(randomX, randomY);
        }

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

    // 조준
    void Aim()
    {
        if (playerInputs.aim)
        {
            isAim = true;

            transform.localEulerAngles = new Vector3(-125f, 0f, 90f);

            rigBuilder.enabled = true; // IK 설정

            // 조준 시점으로 카메라 변경
            aimVirtualCamera.gameObject.SetActive(true);
            playerController.SetSensitivity(aimSensitivity);
            playerController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            animator.SetBool("isAim", true);

            // 조준하는 방향으로 회전
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = base.Master.position.y;
            Vector3 aimDirection = (worldAimTarget - base.Master.position).normalized;

            base.Master.forward = Vector3.Lerp(base.Master.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            isAim = false;
            rigBuilder.enabled = false; // IK 해제

            animator.SetBool("isAim", false);
            transform.localEulerAngles = originalRotation;


            // 원래 시점으로 카메라 변경
            aimVirtualCamera.gameObject.SetActive(false);
            playerController.SetSensitivity(normalSensitivity);
            playerController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        }
    }

    // 격발
    public void Fire()
    {
        if (currentBulletCount <= 0)
        {
            // 총알 없을 때 사격하려하면 자동 장전되게 하려고 했음
            //Debug.Log("재장전시작");
            //animator.SetBool("isReload", true);
            //StartCoroutine(ReloadCoroutine());

            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && isAim && !isReload)
        {
            // 발사 속도 계산
            nextTimeToFire = Time.time + 1f / fireRate;
            
            if (currentBulletCount > 0)
                Shoot();
        }
    }

    // 맞았을 때 효과
    void HitEffect()
    {
        if (blood != null)
        {
            blood.GetHit();
        }
    }

    // 사격
    void Shoot()
    {
        Debug.Log("발사");
        if (hitTransform != null)
        {
            // 무언가 맞았으면
            if (hitTransform.GetComponent<BulletTarget>() != null)
            {
                GameObject GreenEffect = Instantiate(vfxHitGreen, mouseWorldPosition, Quaternion.identity);
                Destroy(GreenEffect, 0.5f);

                HitEffect();
            }
            else
            {
                Instantiate(vfxHitRed, mouseWorldPosition, Quaternion.identity);
            }


            Rigidbody hitRigidbody = hitTransform.GetComponent<Rigidbody>();
            if (hitRigidbody != null)
            {
                // 충격 가할 방향
                Vector3 forceDirection = hitTransform.position - base.Master.position;
                // 충격 적용
                hitRigidbody.AddForce(forceDirection.normalized * 50.0f, ForceMode.Impulse);
            }
        }

        // 탄약 날라가는 로직
        // ProjectBullet();
        
        currentBulletCount--;
        muzzleFlash.Play();
        PlayAudioSource(fireSound);
        // 총기 반동 코루틴 실행
        StartCoroutine(ReactionCoroutine());
        playerInputs.shoot = false;
    }

    // 총알 맞으면 피격 효과
    void HitEffect(RaycastHit hit)
    {
        GameObject Effect = Instantiate(vfxHitGreen, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(Effect, 0.5f);
    }

    // 장전
    void Realod()
    {
        if (playerInputs.reload && !isReload && currentBulletCount < reloadBulletCount)
        {
            Debug.Log("재장전시작");
            animator.SetBool("isReload", true);
            StartCoroutine(ReloadCoroutine());
        }
    }

    // 장전 코루틴
    IEnumerator ReloadCoroutine()
    {
        if (totalBulletCount > 0)
        {
            isReload = true;

            totalBulletCount += currentBulletCount;
            currentBulletCount = 0;

            yield return new WaitForSeconds(reloadTime);

            if (totalBulletCount >= reloadBulletCount)
            {
                currentBulletCount = reloadBulletCount;
                totalBulletCount -= reloadBulletCount;
            }
            else
            {
                currentBulletCount = totalBulletCount;
                totalBulletCount = 0;
            }

            isReload = false;
            playerInputs.reload = false;
            animator.SetBool("isReload", false);
            Debug.Log("재장전 종료");
        }
        else
        {
            animator.SetBool("isReload", false);
        }
    }

    // 반동 코루틴
    IEnumerator ReactionCoroutine()
    {
        isShoot = true;
        yield return new WaitForSeconds(1f);
        isShoot = false;
    }

    // 사격 소리
    void PlayAudioSource(AudioClip _clip)
    {
        _audioSource.clip = _clip;
        _audioSource.Play();
    }

    // 총알 발사
    void ProjectBullet()
    {
        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
    }

    // 총 사용
    public override void Use()
    {
        HitRayCheck();
        Aim();
        Fire();
        Realod();
    }

    public int GetCurrentBullet()
    {
        return currentBulletCount;
    }

    public int GetTotalBullet()
    {
        return totalBulletCount;
    }
}