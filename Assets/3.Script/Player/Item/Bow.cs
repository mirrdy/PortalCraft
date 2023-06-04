using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public int attackDamage; //������
    public float attackRate; //����

    public float arrowForce; //ȭ���� ���ư��� ��
    public float throwUpwardForce; //ȭ���� ���ι߻�Ǵ� ��

    public GameObject player; 
    public GameObject arrowPrefab;
    public Transform arrowSpawn; 
    private GameObject mainCam; 

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Use()
    {
        GameObject Arrow = Instantiate(arrowPrefab, arrowSpawn.position, mainCam.transform.rotation);
        Rigidbody Arrow_Rb = Arrow.GetComponent<Rigidbody>();

        Vector3 forceDirection = mainCam.transform.forward;

        Vector3 forceToAdd = forceDirection * arrowForce + player.transform.up * throwUpwardForce;

        Arrow_Rb.AddForce(forceToAdd, ForceMode.Impulse);
    }
}
