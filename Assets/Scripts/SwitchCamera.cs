using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public GameObject quaterCamera;
    public GameObject thirdCamera;

    public int manager;

    public void ManagerCamera()
    {
        if(manager == 0)
        {
            ThirdView();
            manager = 1;
        }
        else
        {
            QuaterView();
            manager = 0;
        }
    }

    void QuaterView()
    {
        quaterCamera.SetActive(true);
        thirdCamera.SetActive(false);
    }

    void ThirdView()
    {
        quaterCamera.SetActive(false);
        thirdCamera.SetActive(true);
    }
}
