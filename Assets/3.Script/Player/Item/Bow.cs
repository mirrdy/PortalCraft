using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public int attackDamage; //데미지
    public float attackRate; //공속

    public float arrowForce; //화살이 날아가는 힘
    public float throwUpwardForce; //화살이 위로발사되는 힘

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
