using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTest : MonoBehaviour
{
    [SerializeField] MonsterSpawner monsterspawner;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(monsterspawner,transform.position,Quaternion.identity);
        }
    }
}
