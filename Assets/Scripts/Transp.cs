using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transp : MonoBehaviour
{
    public Camera cameraT;
    void Start()
    {
        cameraT.cullingMask = ~(1 << 8);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
