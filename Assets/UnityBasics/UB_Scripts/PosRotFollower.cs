using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosRotFollower : MonoBehaviour
{
    public Transform positionToFollow;
    public Vector3 posAxes = new Vector3(1, 1, 1); // Default to following all axes
    public Transform rotationToFollow;
    public Vector3 rotAxes = new Vector3(1, 1, 1); // Default to following all axes

    void Update()
    {
        if (positionToFollow != null)
        {
            Vector3 targetPos = positionToFollow.position;
            transform.position = new Vector3(
                posAxes.x == 1 ? targetPos.x : transform.position.x,
                posAxes.y == 1 ? targetPos.y : transform.position.y,
                posAxes.z == 1 ? targetPos.z : transform.position.z
            );
        }

        if (rotationToFollow != null)
        {
            Vector3 targetRot = rotationToFollow.eulerAngles;
            transform.rotation = Quaternion.Euler(
                rotAxes.x == 1 ? targetRot.x : transform.eulerAngles.x,
                rotAxes.y == 1 ? targetRot.y : transform.eulerAngles.y,
                rotAxes.z == 1 ? targetRot.z : transform.eulerAngles.z
            );
        }
    }
}
