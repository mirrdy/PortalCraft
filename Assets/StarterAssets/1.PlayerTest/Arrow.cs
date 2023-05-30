using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rigid;
    
    private void Start()
    {
        TryGetComponent(out rigid);
    }

    private void Update()
    {
        transform.forward = rigid.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
