using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public PortalController destPortal;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerControl player))
            {
                if (destPortal.TryGetComponent(out Collider coll))
                {
                    StartCoroutine(SetEnablePortalCollider(coll));
                }
                StartCoroutine(OperatePortal(player));
            }
        }
    }
    IEnumerator SetEnablePortalCollider(Collider coll)
    {
        coll.enabled = false;
        yield return new WaitForSeconds(3f);
        coll.enabled = true;
    }
    IEnumerator OperatePortal(PlayerControl player)
    {
        destPortal.transform.parent.gameObject.SetActive(true);
        while (destPortal.transform.parent.gameObject.activeSelf)
        {
            yield return null;
        }
        player.transform.position = destPortal.transform.position + Vector3.forward;
        transform.parent.gameObject.SetActive(false);
    }
}
