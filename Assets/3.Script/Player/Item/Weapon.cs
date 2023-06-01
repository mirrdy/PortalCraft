using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };

    public Type type;
    public int damage;
    public float rate;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public GameObject arrowPrefab;
    public Transform arrowPoint;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range)
        {
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 50f))
        {

        }


        //GameObject arrow = Instantiate(arrowPrefab, arrowPoint.position, Quaternion.LookRotation(aimDir));
        yield return null;
    }
}
