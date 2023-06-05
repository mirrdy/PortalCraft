using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTestScript : MonoBehaviour
{

    private void Start()
    {
        int fieldItemLayer = LayerMask.NameToLayer("FieldItem");
        int playerLayer = LayerMask.NameToLayer("Player");
        Physics.IgnoreLayerCollision(fieldItemLayer, fieldItemLayer);
        Physics.IgnoreLayerCollision(fieldItemLayer, playerLayer,false);
    }
}
