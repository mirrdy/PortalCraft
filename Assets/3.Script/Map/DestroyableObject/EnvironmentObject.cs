using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour, IDestroyable
{
    public int maxHp;
    public int currentHp;
    public List<FieldItem> dropTables;

    private void Awake()
    {
        currentHp = maxHp;
    }
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            DropItem();
            Destroy(gameObject);
            return;
        }
    }

    public void DropItem()
    {
        if (dropTables.Count > 0)
        {
            Instantiate(dropTables[0], transform.position, Quaternion.identity);
        }
    }
}
