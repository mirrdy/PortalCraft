using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMonster : MonoBehaviour
{
    CharacterController entityController;
    private float gravity = 6;
    // Update is called once per frame
    private void Start()
    {
        entityController = GetComponent<CharacterController>();
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            // 레이가 어떤 객체와 충돌했을 때 null이 아닌 경우에만 이동을 수행합니다
            if (hit.collider == null || hit.distance <= 1f)
            {
                entityController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
            }
        }
    }
}
