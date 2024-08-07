using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 500f;
    public float minZoom = 15f;
    public float maxZoom = 90f;
    public LayerMask terrainLayer; // Layer mask to ensure we only hit terrain colliders
    public float camHeight;

    public float borderThickness = 10f;

    private Camera cam;
    private Rigidbody rb;

    public Transform target; // Target to follow when controlling the tank
    public Vector3 offset = new Vector3(0f, 10f, -10f); // Offset from the target
    public float followSpeed = 5f; // Speed of smoothing

    private Vector3 forceDirection;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (RTSManager.controllingTank)
        {
            HandleTankControl();
        }
        else
        {
            HandleRTSControl();
        }
    }

    private void HandleRTSControl()
    {
        // Calculate force direction based on input
        forceDirection = Vector3.zero;

        // Mouse edge movement
        if (Input.mousePosition.x >= Screen.width - borderThickness)
        {
            forceDirection += transform.right;
        }
        if (Input.mousePosition.x <= borderThickness)
        {
            forceDirection -= transform.right;
        }
        if (Input.mousePosition.y >= Screen.height - borderThickness)
        {
            forceDirection += transform.forward;
        }
        if (Input.mousePosition.y <= borderThickness)
        {
            forceDirection -= transform.forward;
        }

        // Keyboard movement
        if (Input.GetKey("w"))
        {
            forceDirection += transform.forward;
        }
        if (Input.GetKey("s"))
        {
            forceDirection -= transform.forward;
        }
        if (Input.GetKey("a"))
        {
            forceDirection -= transform.right;
        }
        if (Input.GetKey("d"))
        {
            forceDirection += transform.right;
        }

        // Perform a raycast downwards from the camera's position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 200f, terrainLayer))
        {
            camHeight = hit.distance; // Store the height where the ray hits the terrain
        }

        // Zoom in and out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmt = scroll * zoomSpeed * Time.deltaTime;
        Vector3 zoomVector = cam.transform.forward;

        if (scroll != 0)
        {
            // Adjust zoom
            if ((camHeight > minZoom && zoomAmt < 0) || (camHeight < maxZoom && zoomAmt > 0))
            {
                transform.position += zoomVector * zoomAmt;
            }
        }
        else
        {
            // Adjust zoom based on current height
            if (camHeight < minZoom)
            {
                transform.position += zoomVector * (-zoomSpeed * Time.deltaTime);
            }
            else if (camHeight > maxZoom)
            {
                transform.position += zoomVector * (zoomSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleTankControl()
    {
        if (target != null)
        {
            // Calculate the desired position with offset
            Vector3 desiredPosition = target.position + offset;

            // Smoothly interpolate the camera's position towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!RTSManager.controllingTank)
        {
            // Apply forces to the Rigidbody for panning
            if (rb != null)
            {
                rb.AddForce(forceDirection * panSpeed);
            }
        }
    }
}
