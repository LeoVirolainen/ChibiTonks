using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform exitPt;

    private void OnTriggerEnter(Collider other)
    {
        var parent = GetHighestParent(other.gameObject.transform);
        parent.transform.position = exitPt.position;
    }

    Transform GetHighestParent(Transform obj)
    {
        while (obj.parent != null)
        {
            obj = obj.parent;
        }
        return obj;
    }
}
