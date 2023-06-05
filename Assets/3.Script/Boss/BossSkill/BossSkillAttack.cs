using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillAttack : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("쳐맞았당");
        }
    }
}
