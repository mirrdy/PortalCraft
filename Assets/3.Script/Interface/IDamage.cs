using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    //���ط�,������ġ,��������
    void OnDamage(int damage, Vector3 hitPosition, Vector3 hitNomal);
}
