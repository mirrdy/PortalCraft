using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazierTest : MonoBehaviour
{
    [Range(0, 1)]
    public float TestValue;

    public Transform ArrowPoint_1;
    public Transform ArrowPoint_2;
    public Transform ArrowPoint_3;
    public Transform ArrowPoint_4;

    private void Update()
    {
        transform.position = Bazier(ArrowPoint_1.position, ArrowPoint_2.position, ArrowPoint_3.position, ArrowPoint_4.position, TestValue);
    }

    private Vector3 Bazier(Vector3 p_1, Vector3 p_2, Vector3 p_3, Vector3 p_4, float value)
    {
        Vector3 A = Vector3.Lerp(p_1, p_2, value);
        Vector3 B = Vector3.Lerp(p_2, p_3, value);
        Vector3 C = Vector3.Lerp(p_3, p_4, value);

        Vector3 D = Vector3.Lerp(A, B, value);
        Vector3 E = Vector3.Lerp(B, C, value);

        Vector3 F = Vector3.Lerp(D, E, value);

        return F;
    }
}
