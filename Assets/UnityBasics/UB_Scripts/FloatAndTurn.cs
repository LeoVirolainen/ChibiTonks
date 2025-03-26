using UnityEngine;

public class FloatAndTurn : MonoBehaviour
{
    // Variables for floating behavior
    [Tooltip("How high/low to float")]
    public float floatAmplitude = 0.5f;   // Maximum vertical displacement
    [Tooltip("Float speed")]
    public float floatFrequency = 1f;     // Speed of vertical movement
    [Tooltip("Spin speed")]
    public float rotationSpeed = 45f;     // Speed of rotation around Y-axis (degrees per second)
    [Tooltip("Axis to spin along")]
    public Vector3 rotationAxis = Vector3.up;

    // Initial local position to keep track of starting point
    private Vector3 startLocalPosition;

    void Start()
    {
        // Record the starting local position of the object
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Calculate the new local vertical position using a sine wave
        float newY = startLocalPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(startLocalPosition.x, newY, startLocalPosition.z);

        // Rotate the object around the set axis
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
