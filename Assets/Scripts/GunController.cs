using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun _currentGun;

    private float _currentFireRate;
    private bool _isReload = false;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        GunFireRateCalc();
        TryFire();
    }

    private void GunFireRateCalc()
    {
        if (_currentFireRate > 0)
        {
            _currentFireRate -= Time.deltaTime;
        }
    }

    private void TryFire()
    {
        if (Input.GetButton("Fire1") && _currentFireRate <= 0) // + && !_isReload
        {
            Fire();
        }
    }

    private void Fire() // Before shoot
    {
        if(_currentGun.currentBulletCount > 0)
        {
            Shoot();
        }
        else
        {
            Reload();
        }
    }

    private void Shoot() // After shoot
    {
        _currentGun.currentBulletCount--;
        _currentFireRate = _currentGun.fireRate;
        _currentGun.muzzleFlash.Play();
        PlayAudioSource(_currentGun.fireSound);
        Debug.Log("Shoot");
    }

    private void Reload()
    {
        if(_currentGun.carryBulletCount > 0)
        {
            // _currentGun.anim.SetTrigger("Reload");

        // yield return new WaitForSeconds(_currentGun.reloadTime);

        if(_currentGun.carryBulletCount >= _currentGun.reloadBulletCount)
        {
            _currentGun.currentBulletCount = _currentGun.reloadBulletCount;
            _currentGun.carryBulletCount -= _currentGun.reloadBulletCount;
        }
        else
        {
            _currentGun.currentBulletCount = _currentGun.carryBulletCount;
            _currentGun.carryBulletCount = 0;
        }

        }
    }

    private void PlayAudioSource(AudioClip _clip)
    {
        _audioSource.clip = _clip;
        _audioSource.Play();
    }

}
