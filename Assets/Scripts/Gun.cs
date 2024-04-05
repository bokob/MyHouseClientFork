using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// two hand, range weapon
/// </summary>
public class Gun : Weapon
{
    [SerializeField]
    public float range;
    public float reloadTime;

    public int reloadBulletCount;
    public int currentBulletCount;
    public int maxBulletCount;
    public int carryBulletCount;

    public float retroActionForce;
    public float retroActionFineSightForce;
    public Vector3 fineSightOriginPos;
    public Animator anim;

    public ParticleSystem muzzleFlash;
    public AudioClip fireSound;

    //public Define.Type Type { get; protected set; } // Weapon type

    //public int Attack { get; protected set; }       // Damage
    //public float Rate { get; protected set; }       // Attack rate

    private float _currentFireRate;
    private bool _isReload = false;
    private bool _isFineSightMode = false;

    [SerializeField]
    private Vector3 _originPos;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        GunRateCalc();
        TryFire();
        TryFineSight();
    }

    private void GunRateCalc()
    {
        if (_currentFireRate > 0)
        {
            _currentFireRate -= Time.deltaTime;
        }
    }

    private void TryFire()
    {
        if (Input.GetButton("Fire1") && _currentFireRate <= 0 && !_isReload)
        {
            Fire();
        }
    }

    private void Fire() // Before shoot
    {
        if(!_isReload)
        {
            if(currentBulletCount > 0)
            {
                Shoot();
            }
            else
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
        
    }

    private void Shoot() // After shoot
    {
        currentBulletCount--;
        _currentFireRate = base.Rate;
        muzzleFlash.Play();
        PlayAudioSource(fireSound);
        Debug.Log("Shoot");
    }

    IEnumerator ReloadCoroutine()
    {
        if(carryBulletCount > 0)
        {
            _isReload = true;
            // anim.SetTrigger("Reload");

            carryBulletCount += currentBulletCount;
            currentBulletCount = 0;

            yield return new WaitForSeconds(reloadTime);

            if(carryBulletCount >= reloadBulletCount)
            {
                currentBulletCount = reloadBulletCount;
                carryBulletCount -= reloadBulletCount;
            }
            else
            {
                currentBulletCount = carryBulletCount;
                carryBulletCount = 0;
            }
            
            _isReload = false;
        }
    }

    private void TryFineSight()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            FineSight();
        }
    }

    private void FineSight()
    {
        _isFineSightMode = !_isFineSightMode;
        //anim.SetBool("", _isFineSightMode);
        
        if(_isFineSightMode)
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

    IEnumerator FineSightActivateCoroutine() // Activate
    {
        while(transform.localPosition != fineSightOriginPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, fineSightOriginPos, 0.2f);
            yield return null; // Stand by 1 frame at a time.
        }
    }

    IEnumerator FineSightDeactivateCoroutine() // Deactivate
    {
        while(transform.localPosition != _originPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originPos, 0.2f);
            yield return null; // Stand by 1 frame at a time.
        }
    }

    private void PlayAudioSource(AudioClip _clip)
    {
        _audioSource.clip = _clip;
        _audioSource.Play();
    }

}
