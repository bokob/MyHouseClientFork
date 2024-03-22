using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    // Start is called before the first frame update
    Status _status;
    Animator _anim;
    void Start()
    {
        gameObject.AddComponent<Status>();
        _status = gameObject.GetComponent<Status>();
        _anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("´ê¾Ò´Ù");
        if (other.tag == "Melee")
        {
            _anim.SetBool("isDead", true); 
        }
    }
}
