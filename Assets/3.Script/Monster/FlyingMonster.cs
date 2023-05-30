using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMonster : MonoBehaviour
{
    MonsterControl monster;
    private float gravity = 6;
    // Update is called once per frame
    private void Start()
    {
        monster = GetComponent<MonsterControl>();
    }
    void Update()
    {
        //if (monster.entityController.isGrounded)
        //{
        //    monster.entityController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        //}
        //RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Monster");
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f, layerMask))
        {
            monster.entityController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        }
        else
        {
            // �浹 ��ü�� ������ �߷��� �����Ͽ� �Ʒ��� �̵�
            monster.entityController.Move(new Vector3(0, -gravity, 0) * Time.deltaTime);
        }
        //if (monster.entityController.isGrounded)
        //{
        //    monster.entityController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        //}
        //if (!monster.entityController.isGrounded)
        //{
        //    monster.entityController.Move(new Vector3(0, -gravity, 0) * Time.deltaTime);
        //}

    }
}
