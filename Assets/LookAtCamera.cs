using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public RectTransform uiElement; // The RectTransform you want to rotate

    void Update()
    {
        // Make sure to use the world position of the camera
        Vector3 directionToCamera = Camera.main.transform.position - uiElement.position;
        Quaternion rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);

        // Apply the rotation
        uiElement.rotation = rotation;
    }
}
