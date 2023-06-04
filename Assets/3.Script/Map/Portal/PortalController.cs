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

                destPortal.transform.parent.gameObject.SetActive(true);
                player.transform.position = destPortal.transform.position + Vector3.forward;
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
    IEnumerator SetEnablePortalCollider(Collider coll)
    {
        coll.enabled = false;
        yield return new WaitForSeconds(3f);
        coll.enabled = true;
    }
}
