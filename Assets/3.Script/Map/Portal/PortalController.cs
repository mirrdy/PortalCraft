using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private PortalController destPortal;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(other.TryGetComponent(out PlayerController player))
            {
                player.transform.position = destPortal.transform.position + Vector3.forward;
            }
            
        }
    }
}
