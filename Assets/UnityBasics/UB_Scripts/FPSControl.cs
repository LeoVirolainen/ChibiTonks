using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSControl : MonoBehaviour
{

    [Header("Camera Settings")]
    public float minX = -60f; // Lower limit for vertical camera rotation
    public float maxX = 60f; // Upper limit for vertical camera rotation
    public float sensitivity = 2f; // Mouse sensitivity
    public Camera cam; // Reference to the player camera

    [Header("Zoom Settings")]
    public float zoomTargetFOV = 30f; // Field of view when zooming
    public float camLerpTime = 0.15f; // Speed of zoom transition
    private float originalFOV; // Stores default FOV value

    [Header("Movement Settings")]
    public float walkSpeed = 5f; // Walking speed
    public float runMultiplier = 1.5f; // Speed multiplier when sprinting

    private Rigidbody rb; // Rigidbody reference for physics-based movement
    private bool moving; // Whether the player is moving

    // Variables to track camera rotation
    private float yRot = 0f;
    private float xRot = 0f;

    void Start()
    {
        // Multiply the walk speed for fine-tuning
        walkSpeed *= 10;

        // Lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Store the original field of view for zoom functionality
        originalFOV = cam.fieldOfView;
    }

    void Update()
    {
        LookRotations(); // Handle player looking around
        CheckMoving(); // Track movement state
        CamZoom(); // Handle zooming
    }

    void FixedUpdate()
    {
        if (moving)
        {
            Movement();
        }
    }

    // Handles mouse look rotation
    void LookRotations()
    {
        yRot += Input.GetAxis("Mouse X") * sensitivity; // Horizontal rotation
        xRot += Input.GetAxis("Mouse Y") * sensitivity; // Vertical rotation

        xRot = Mathf.Clamp(xRot, minX, maxX); // Limit vertical rotation

        transform.rotation = Quaternion.Euler(0, yRot, 0);
        cam.transform.localRotation = Quaternion.Euler(-xRot, 0, 0);
    }

    // Handles movement input
    void Movement()
    {
        float xMove = Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime;
        float zMove = Input.GetAxis("Vertical") * walkSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift))
        { // Sprinting
            zMove *= runMultiplier;
        }

        Vector3 movement = new Vector3(xMove, rb.velocity.y, zMove);
        rb.velocity = transform.TransformDirection(movement);
    }

    // Checks if the player is moving
    void CheckMoving()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    // Handles zooming in and out
    void CamZoom()
    {
        if (Input.GetMouseButton(1))
        { // Right mouse button zooms in
            LerpFOV(zoomTargetFOV);
            CameraShake.Instance.Shake(0.2f, 0.05f);
        }
        else
        {
            LerpFOV(originalFOV);
        }
    }

    // Smoothly transitions the field of view
    void LerpFOV(float target)
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, target, Time.deltaTime / camLerpTime);
    }
}
