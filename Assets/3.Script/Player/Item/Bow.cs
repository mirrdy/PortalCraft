using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public int attackDamage;
    public float attackRate;

    public GameObject arrowPrefab;
    public Transform arrowPoint;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Use()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        ray.GetPoint

        Instantiate(arrowPrefab, arrowPoint.position, mainCamera.transform.rotation);
    }
}
