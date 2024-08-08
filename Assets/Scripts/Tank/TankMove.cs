using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMove : MonoBehaviour
{
    public float maxSpeed = 20f;
    public float acceleration = 5f;
    public float deceleration = 10f;
    public float turnSpeed = 100f;

    private Rigidbody rb;
    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (RTSManager.controllingTank)
        {
            // Get input for moving forward/backward
            float moveInput = Input.GetAxis("Vertical"); // W and S or UpArrow and DownArrow
                                                         // Get input for turning left/right
            float turnInput = Input.GetAxis("Horizontal"); // A and D or LeftArrow and RightArrow

            // Calculate acceleration and braking
            if (moveInput > 0) // Accelerating forward
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
            else if (moveInput < 0) // Reversing
            {
                currentSpeed -= acceleration * Time.deltaTime;
            }
            else // No input, apply braking
            {
                if (currentSpeed > 0)
                    currentSpeed -= deceleration * Time.deltaTime;
                else if (currentSpeed < 0)
                    currentSpeed += deceleration * Time.deltaTime;
            }

            // Clamp the current speed to the max speed
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

            // Get movement and turning values
            Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
            Quaternion turn = Quaternion.Euler(0f, turnInput * turnSpeed * Time.deltaTime, 0f);

            // Apply movement and turning
            rb.MovePosition(rb.position + move);
            rb.MoveRotation(rb.rotation * turn);
        }
    }
}
