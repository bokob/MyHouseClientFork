using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// two hand, range weapon
/// </summary>
public class Gun : Weapon
{
    #region 총 관련 변수
    [Tooltip("사거리")]
    [SerializeField] float range;
    [Tooltip("재장전 시간")]
    [SerializeField] float reloadTime;
    [Tooltip("한 번 장전 시, 장전할 탄약 수")]
    [SerializeField] int reloadBulletCount;
    [Tooltip("현재 탄약 수")]
    [SerializeField] int currentBulletCount;
    [Tooltip("최대 탄약 수")]
    [SerializeField] int maxBulletMagazine;
    [Tooltip("총 탄약 수")]
    [SerializeField] int totalBulletCount;

    [Tooltip("조준 반동")]
    [SerializeField] float reactionForce;
    [Tooltip("정조준 반동")]
    [SerializeField] float reactionFineSightForce;

    [SerializeField] AudioSource _audioSource;
    [Tooltip("사격 효과")]
    [SerializeField] ParticleSystem muzzleFlash;
    [Tooltip("총 소리")]
    [SerializeField] AudioClip fireSound;

    [Tooltip("발사 속도")]
    [SerializeField] float _currentFireRate;
    [Tooltip("재장전 중인지 여부")]
    [SerializeField] bool _isReload = false;
    [Tooltip("정조준 중인지 여부")]
    [SerializeField] bool _isFineSightMode = false;

    [SerializeField] Animator anim;
    [Tooltip("원래 카메라 위치")]
    [SerializeField] Vector3 _originPos;
    [Tooltip("정조준 카메라 위치")]
    [SerializeField] Vector3 fineSightOriginPos;
    #endregion

    Animator houseownerAnim;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public override void Use()
    {
        GunRateCalc();
        Fire();
        FineSight();
    }

    void GunRateCalc()
    {
        if (_currentFireRate > 0)
        { 
            _currentFireRate -= Time.deltaTime;
            return;
        }
    }

    /// <summary>
    /// 격발
    /// </summary>
    public void Fire()
    {
        if (Input.GetButton("Fire1") && _currentFireRate <= 0 && !_isReload)
        {
            if (currentBulletCount > 0)
                Shoot();
            else
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    /// <summary>
    /// 사격
    /// </summary>
    void Shoot() // After shoot
    {
        currentBulletCount--;
        _currentFireRate = base.Rate;
        muzzleFlash.Play();
        PlayAudioSource(fireSound);
        // 총기 반동 코루틴 실행
        StopAllCoroutines();
       

        Debug.Log("Shoot");
    }

    void Realod()
    {
        if(Input.GetKeyDown(KeyCode.R) && !_isReload && currentBulletCount < reloadBulletCount)
        {
            // 재장전 해제
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if (totalBulletCount > 0)
        {
            _isReload = true;

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

            _isReload = false;
        }
    }

    public void FineSight()
    {
        if (Input.GetMouseButton(1))
        {
            _isFineSightMode = !_isFineSightMode;
            
            // 정조준 애니메이션 넣을 예정
            //anim.SetBool("", _isFineSightMode);

            if (_isFineSightMode)
            {
                StopAllCoroutines();
                StartCoroutine(FineSightActivateCoroutine());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(FineSightDeactivateCoroutine());
            }
        }
    }

    IEnumerator FineSightActivateCoroutine() // Activate
    {
        while (transform.localPosition != fineSightOriginPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, fineSightOriginPos, 0.2f);
            yield return null; // Stand by 1 frame at a time.
        }
    }

    IEnumerator FineSightDeactivateCoroutine() // Deactivate
    {
        while (transform.localPosition != _originPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originPos, 0.2f);
            yield return null; // Stand by 1 frame at a time.
        }
    }

    IEnumerator ReactionCoroutine()
    {
        Vector3 reactionNormal = new Vector3(reactionForce, _originPos.y, _originPos.z);     // 정조준 안 했을 때의 최대 반동
        Vector3 reactionFineSight = new Vector3(reactionFineSightForce, fineSightOriginPos.y, fineSightOriginPos.z);  // 정조준 했을 때의 최대 반동

        if (!_isFineSightMode)  // 정조준이 아닌 상태
        {
            transform.localPosition = _originPos;

            // 반동 시작
            while (transform.localPosition.x <= reactionForce - 0.02f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, reactionNormal, 0.4f);
                yield return null;
            }

            // 원위치
            while (transform.localPosition != _originPos)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _originPos, 0.1f);
                yield return null;
            }
        }
        else  // 정조준 상태
        {
            transform.localPosition = fineSightOriginPos;

            // 반동 시작
            while (transform.localPosition.x <= reactionFineSightForce - 0.02f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, reactionFineSight, 0.4f);
                yield return null;
            }

            // 원위치
            while (transform.localPosition != fineSightOriginPos)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    void PlayAudioSource(AudioClip _clip)
    {
        _audioSource.clip = _clip;
        _audioSource.Play();
    }
}