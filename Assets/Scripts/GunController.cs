using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun _currentGun;

    private float _currentFireRate;

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
        if (Input.GetButton("Fire1") && _currentFireRate <= 0)
        {
            Fire();
        }
    }

    private void Fire()
    {
        _currentFireRate = _currentGun.fireRate;
        Shoot();
    }

    private void Shoot()
    {
        _currentGun.muzzleFlash.Play();
        PlayAudioSource(_currentGun.fireSound);
        Debug.Log("Shoot");
    }

    private void PlayAudioSource(AudioClip _clip)
    {
        _audioSource.clip = _clip;
        _audioSource.Play();
    }

}
