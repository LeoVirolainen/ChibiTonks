using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosRotFollower : MonoBehaviour
{
    public Transform positionToFollow;
    public Transform rotationToFollow;
    void Start()
    {
        
    }
    void Update()
    {
        if (positionToFollow != null)
        transform.position = positionToFollow.position;
        if (rotationToFollow != null)
            transform.rotation = rotationToFollow.rotation;
    }
}
