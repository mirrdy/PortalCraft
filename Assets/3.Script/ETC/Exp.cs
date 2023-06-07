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
    
    //����ġ ������ ������ �����ð��ڿ� ������� �ڷ�ƾ���
    private void Start()
    {
        StartCoroutine(Exp_co());
    }
    //�÷��̾� �±׿� ���˽� ����ġȹ��
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
