using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public PortalController destPortal;
    public float moveProgress;

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
        moveProgress = 0;

        LoadingSceneManager.Instance.LoadPortal(this);
        yield return new WaitForSeconds(1f);
        destPortal.transform.parent.gameObject.SetActive(true);

        int lastIndex = destPortal.transform.parent.childCount-1;
        int unit = lastIndex / 100;

        for(int i=0; i<100; i++)
        {
            Debug.Log(Time.realtimeSinceStartup);
            Transform child = destPortal.transform.parent.GetChild(i * unit);
            while (!child.gameObject.activeSelf)
            {
                yield return null;
            }
            moveProgress++;
        }

        Transform lastChild = destPortal.transform.parent.GetChild(lastIndex);
        while (!lastChild.gameObject.activeSelf)
        {
            yield return null;
        }
        moveProgress = 100;

        player.TryGetComponent(out CharacterController controller);
        controller.enabled = false;
        
        player.transform.position = destPortal.transform.position + Vector3.forward;

        controller.enabled = true;
        transform.parent.gameObject.SetActive(false);
    }
}
