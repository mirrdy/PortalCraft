using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObject : MonoBehaviour, IDestroyable
{
    public int maxHp;
    public int currentHp;
    public List<FieldItem> dropTables;

    private void Awake()
    {
        currentHp = maxHp;
    }
    private void Update()
    {
        //TakeDamage(5);
    }
    public void TakeDamage(int damage)
    {
        if (currentHp <= 0)
        {
            DropItem();
            Destroy(gameObject);
            return;
        }
        currentHp -= damage;
    }

    public void DropItem()
    {
        if (dropTables.Count > 0)
        {
            Instantiate(dropTables[0], transform.position, Quaternion.identity);
        }
    }
    
}