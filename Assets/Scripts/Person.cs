using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    Status _status;
    Animator _anim;
    void Start()
    {
        gameObject.AddComponent<Status>();
        _status = gameObject.GetComponent<Status>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("닿았다");

        /*
         * 근접 무기에만 적용됨
         * 나중에 원거리 무기에 피격 당했을 때의 일도 처리해 줘야 함
         * Tag를 이용하기 보다는 Weapon.cs에 담긴 Type으로 구분할 예정 
         */
        if(other.tag == "Melee")
        {
            _status.TakedDamage(other.GetComponent<Weapon>().Attack);
            
            if(_status.Hp <= 0)
                _anim.SetBool("isDead", true);
        }
    }
}
