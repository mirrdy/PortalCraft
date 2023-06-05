using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDestroyable obj))
        {
            obj.TakeDamage(50000000);
        }
        else if(other.TryGetComponent(out PlayerControl player))
        {
            player.OnDamage(50000000, other.transform.position, other.transform.position);
        }
    }
}
