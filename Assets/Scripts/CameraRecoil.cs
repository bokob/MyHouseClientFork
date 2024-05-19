using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float maxRecoilAngleY = 5f; // Y축 최대 반동 각도
    public float maxRecoilAngleX = 2f; // X축 최대 반동 각도
    public float recoilSpeed = 10f; // 반동이 발생하는 속도
    public float returnSpeed = 20f; // 원래 위치로 돌아오는 속도

    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    private Vector3 originalRotation;

    private void Start()
    {
        originalRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        // 천천히 원래 위치로 복귀
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, Time.deltaTime * returnSpeed);
        transform.localEulerAngles = originalRotation + currentRecoil;
    }

    /// <summary>
    /// 총 발사 시 반동을 적용하는 메서드
    /// </summary>
    /// <param name="recoilAmountX">X축 반동</param>
    /// <param name="recoilAmountY">Y축 반동</param>
    public void ApplyRecoil(float recoilAmountX, float recoilAmountY)
    {
        recoilAmountX = Mathf.Clamp(recoilAmountX, -maxRecoilAngleX, maxRecoilAngleX);
        recoilAmountY = Mathf.Clamp(recoilAmountY, -maxRecoilAngleY, maxRecoilAngleY);

        targetRecoil += new Vector3(-recoilAmountX, recoilAmountY, 0); // X축은 음수로 적용
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilSpeed);
    }
}
