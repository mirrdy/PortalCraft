using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyable
{
    public void DropItem();
    public void TakeDamage(int damage);
}
