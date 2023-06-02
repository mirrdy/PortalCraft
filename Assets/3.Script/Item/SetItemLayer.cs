using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetItemLayer : MonoBehaviour
{
    private void Awake()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        Physics.IgnoreLayerCollision(gameObject.layer, playerLayer);
    }
}
