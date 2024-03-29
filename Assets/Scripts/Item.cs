using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float floatHeight = 0.5f; // 아이템의 떠다니는 높이
    public float floatSpeed = 1.0f;  // 아이템의 떠다니는 속도
    public float rotateSpeed = 30f;  // 아이템의 회전 속도
    public float floatScale = 0.1f;  // Sin 함수의 반환 값에 곱해줄 스케일링 팩터, 완만하게 움직이게 하려고 사용

    Transform childMesh;
    void Start()
    {
        // Mesh를 가져온다.
        childMesh = transform.GetChild(0);
    }

    void Update()
    {
        // 아이템을 회전 (월드 좌표 기준)
        // 자기 위치를 기점으로 월드 좌표 방향(Vector3.up) 회전
        childMesh.RotateAround(childMesh.position, Vector3.up, rotateSpeed * Time.deltaTime);
        
        // childMesh.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        // 아이템이 위아래로 떠다닐 높이
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatScale + floatHeight; 
        childMesh.position = new Vector3(childMesh.position.x, newY, childMesh.position.z);
    }
}
