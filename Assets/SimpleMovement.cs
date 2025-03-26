using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float maxMoveSpeed = 5.0f; // Maximum speed of forward movement
    public float maxTurnSpeed = 100.0f; // Maximum speed of turning
    public float acceleration = 2.0f; // How quickly the object accelerates
    public float deceleration = 2.0f; // How quickly the object decelerates

    private float currentMoveSpeed = 0.0f; // Current speed of forward movement
    private float currentTurnSpeed = 0.0f; // Current speed of turning

    void Update()
    {
        // Handle forward movement with W
        if (Input.GetKey(KeyCode.W))
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, maxMoveSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, 0, Time.deltaTime * deceleration);
        }

        // Handle turning with A
        if (Input.GetKey(KeyCode.A))
        {
            currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, -maxTurnSpeed, Time.deltaTime * acceleration);
        }
        // Handle turning with D
        else if (Input.GetKey(KeyCode.D))
        {
            currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, maxTurnSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, 0, Time.deltaTime * deceleration);
        }

        // Apply the movement and rotation
        transform.Translate(Vector3.forward * currentMoveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, currentTurnSpeed * Time.deltaTime);
    }
}
