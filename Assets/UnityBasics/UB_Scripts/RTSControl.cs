using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSControl : MonoBehaviour
{
    // Movement parameters
    public float maxHeight;
    public float minHeight;
    public float moveSpeed;
    public float sensitivity;
    // Variables to track camera rotation
    private float yRot = 0f;
    private float xRot = 0f;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float moveY = 0; // Default to 0
        if (Input.GetKey(KeyCode.E))
            if (transform.position.y < maxHeight)
                moveY = .3f;
        if (Input.GetKey(KeyCode.Q))
            if (transform.position.y > minHeight)
                moveY = -.3f;

        transform.Translate(new Vector3(moveX, moveY, moveZ) * moveSpeed);        

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            yRot += Input.GetAxis("Mouse X"); // Horizontal rotation
            xRot += Input.GetAxis("Mouse Y"); // Vertical rotation

            xRot = Mathf.Clamp(xRot, -60, 0); // Limit vertical rotation

            transform.localEulerAngles = new Vector3(0, yRot, 0) * sensitivity; // Rotate the player
            cam.transform.localEulerAngles = new Vector3(-xRot, 0, 0) * sensitivity; // Rotate the camera
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
