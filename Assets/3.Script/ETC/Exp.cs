using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    private int exp = 5;
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.CompareTag("Player"))
    //    {
    //        collision.transform.TryGetComponent(out PlayerControl player);
    //        player.GetExp(exp);
    //        Destroy(gameObject);
    //    }
    //}
    
    //경험치 아이템 생성시 일정시간뒤에 사라지는 코루틴사용
    private void Start()
    {
        StartCoroutine(Exp_co());
    }
    //플레이어 태그와 접촉시 경험치획득
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.TryGetComponent(out PlayerControl player);
            player.GetExp(exp);
            Destroy(transform.parent.gameObject);
        }
    }
    private IEnumerator Exp_co()
    {
        yield return new WaitForSeconds(90f);
        Destroy(transform.parent.gameObject);
    }
}
