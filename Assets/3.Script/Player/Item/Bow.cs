using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public int attackDamage;
    public float attackRate;
    public float attackRange = 1000f;

    public GameObject arrowPrefab;
    public Transform arrowPoint;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Use()
    {
        Vector3 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, attackRange))
        {
            Vector3 aimDir = hitInfo.point - arrowPoint.position;
            Instantiate(arrowPrefab, arrowPoint.position, Quaternion.LookRotation(aimDir));
        }
        //Quaternion.Euler
        //Physics.Raycast()
    }
}
