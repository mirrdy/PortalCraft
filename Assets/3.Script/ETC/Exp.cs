using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    private int exp = 5;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.TryGetComponent(out PlayerControl player);
            player.GetExp(exp);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("트리거 실행됨");
            other.transform.TryGetComponent(out PlayerControl player);
            player.GetExp(exp);
            Destroy(gameObject);
        }
    }
}
