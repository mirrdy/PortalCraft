using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float arrowSpeed = 20f;
    private Rigidbody rigid;
    
    private void Start()
    {
        TryGetComponent(out rigid);
        rigid.velocity = transform.forward * arrowSpeed;
    }

    private void Update()
    {
        transform.forward = rigid.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
