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
                    //StartCoroutine(SetEnablePortalCollider(coll));
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

        int lastIndex = destPortal.transform.parent.childCount-1;
        Transform lastChild = destPortal.transform.parent.GetChild(lastIndex);

        while (!lastChild.gameObject.activeSelf)
        {
            yield return null;
        }
        player.TryGetComponent(out CharacterController controller);
        controller.enabled = false;
        
        Debug.Log($"목표포탈 위치: {destPortal.transform.position}");
        player.transform.position = destPortal.transform.position + Vector3.forward;
        Debug.Log($"플레이어 위치: {player.transform.position}");

        controller.enabled = true;
        transform.parent.gameObject.SetActive(false);
    }
}
