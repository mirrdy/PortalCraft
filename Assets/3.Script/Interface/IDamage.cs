using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    //피해량,맞은위치,맞은각도
    void OnDamage(float damage, Vector3 hitPosition, Vector3 hitNomal);
}
