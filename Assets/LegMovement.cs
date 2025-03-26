using UnityEngine;

public class LegMovement : MonoBehaviour
{
    public Transform body; // Reference to the body of the walker
    public LegMovement oppositeLeg; // Opposing leg whose movements to watch for
    public float forwardDistance = 1.2f; // Desired distance in the forward direction from the body
    public float sideDistance = 0.5f; // Desired distance in the side direction from the body
    public float moveSpeed = 20f; // Speed at which the leg moves to the new position  
    public float maxDistance = 1.5f; // Maximum allowable distance before the leg needs to move
    public float maxHeight; // Maximum height to raise leg while walking

    private Vector3 targetPosition;
    private float targetHalfwayDist;
    private bool isMoving = false;

    private float startMoveSpeed;

    private void Start()
    {
        startMoveSpeed = moveSpeed;
    }

    void Update()
    {
        // Check if the leg is too far from the body
        if (!isMoving && IsLegTooFar() && !oppositeLeg.isMoving)
        {
            // Set the new target position for the leg
            SetNewTargetPosition();
            isMoving = true;
        }

        // Move the leg to the target position if it's moving
        if (isMoving)
        {
            MoveLeg();
        }
    }

    bool IsLegTooFar()
    {
        // Check if the leg is beyond the maximum allowable distance in any direction
        Vector3 offset = transform.position - body.position;
        return offset.magnitude > maxDistance;
    }

    void SetNewTargetPosition()
    {
        moveSpeed = startMoveSpeed;

        var localForwardDistance = forwardDistance;
        var localSideDistance = sideDistance;
        var localMoveSpd = moveSpeed;

        if (Input.GetKey(KeyCode.W))
        {
            localForwardDistance = forwardDistance + 1.2f;
            localMoveSpd = moveSpeed *= 1.1f;
        }
        else
        {
            localForwardDistance = forwardDistance;
            localMoveSpd = moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (forwardDistance > 0) //check if this is a front leg
                localSideDistance = sideDistance - 0.5f;
            else
                localSideDistance = sideDistance + 0.5f;
            localMoveSpd = moveSpeed *= 1.25f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (forwardDistance > 0) //check if this is a front leg
                localSideDistance = sideDistance + 0.5f;
            else
                localSideDistance = sideDistance - 0.5f;
            localMoveSpd = moveSpeed *= 1.25f;
        }
        else
        {
            localSideDistance = sideDistance;
            localMoveSpd = moveSpeed;
        }
        // Set the new target position directly based on desired distances
        targetPosition = new Vector3(body.position.x, body.position.y - 1.2f, body.position.z) + (body.forward * (localForwardDistance)) + (body.right * localSideDistance);
        moveSpeed = localMoveSpd;
        //targetHalfwayDist = Vector3.Distance(transform.position, targetPosition) / 2;
        //transform.position = new Vector3(transform.position.x, transform.position.y + maxHeight, transform.position.z);
    }

    void MoveLeg()
    {        
        var localTargetPos = targetPosition;
        /*
        if (Vector3.Distance(transform.position, targetPosition) > targetHalfwayDist) //if less than halfway through
        {
            localTargetPos = new Vector3(targetPosition.x, targetPosition.y + maxHeight, targetPosition.z);
        }
        else
        {
            localTargetPos = targetPosition;
        }*/
        
        
        // Move the leg to the target position smoothly
        transform.position = Vector3.Lerp(transform.position, localTargetPos, Time.deltaTime * moveSpeed);

        // Check if the leg has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false; // Stop moving when the leg reaches the target position
        }
    }
}
