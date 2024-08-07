using UnityEngine;

public class TankCameraController : MonoBehaviour
{
    public Transform target; // Target object to follow
    public Vector3 offset = new Vector3(0f, 10f, -10f); // Offset from the target
    public float followSpeed = 5f; // Speed of smoothing

    public float panSpeed = 20f;
    public float borderThickness = 10f;

    private void LateUpdate()
    {
        if (RTSManager.controllingTank)
        {
            if (target == null) return;

            // Calculate the desired position with offset
            Vector3 desiredPosition = target.position + offset;

            // Smoothly interpolate the camera's position towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }

    private void Update()
    {
        if (!RTSManager.controllingTank)
        {
            Vector3 movement = Vector3.zero;

            // Mouse edge movement
            if (Input.mousePosition.x >= Screen.width - borderThickness)
            {
                movement += transform.right;
            }
            if (Input.mousePosition.x <= borderThickness)
            {
                movement -= transform.right;
            }
            if (Input.mousePosition.y >= Screen.height - borderThickness)
            {
                movement += transform.forward;
            }
            if (Input.mousePosition.y <= borderThickness)
            {
                movement -= transform.forward;
            }

            // Keyboard movement
            if (Input.GetKey("w"))
            {
                movement += transform.forward;
            }
            if (Input.GetKey("s"))
            {
                movement -= transform.forward;
            }
            if (Input.GetKey("a"))
            {
                movement -= transform.right;
            }
            if (Input.GetKey("d"))
            {
                movement += transform.right;
            }

            // Move the camera
            transform.position += movement * panSpeed * Time.deltaTime;
        }
    }
}
