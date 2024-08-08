using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotExplosion : MonoBehaviour
{
    public float explRadius;
    void Start()
    {
        Collider[] enemiesToKill = Physics.OverlapSphere(transform.position, explRadius);
        foreach (Collider enemy in enemiesToKill)
        {
            if (enemy.GetComponent<EnemySoldier>() != null)
            {
                enemy.GetComponent<EnemySoldier>().Die();
            }
        }
    }
}
