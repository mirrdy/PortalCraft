using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObject : MonoBehaviour, IDestroyable
{
    public int maxHp;
    public int currentHp;
    public bool isCreatedByGenerator;
    public List<FieldItem> dropTables;

    private void Awake()
    {
        currentHp = maxHp;
    }
    public void TakeDamage(int damage)
    {
        AudioManager.instance.PlaySFX("PlayerDestroyBlock");
        currentHp -= damage;
        if (currentHp <= 0)
        {
            DropItem();
            if (isCreatedByGenerator)
            {
                int.TryParse(transform.parent.name.Substring(6), out int islandIndex);
                BlockMapGenerator.instance.CheckAroundDestroyedBlock(islandIndex - 1, transform.position);
            }
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
