using UnityEngine;

public class TurretControl : MonoBehaviour
{
    public Transform turret; // The turret's transform
    public Transform barrel; // The barrel's transform
    public Transform lineStart; // The exit point of the barrel
    public Transform targetObject; // The object that will follow the target point
    public float rotationSpeed = 5f; // Speed of the turret rotation
    public Camera mainCamera; // Reference to the main camera
    public float maxRaycastDistance = 1000f; // Maximum distance for raycast
    public LineRenderer lineRenderer; // Reference to the LineRenderer component

    // Initial rotation offsets for the turret and barrel
    public Vector3 turretRotationOffset = new Vector3(-90f, 0f, 180f);
    public Vector3 barrelInitialRotation = new Vector3(-90f, 0f, 0f);

    void Start()
    {
        // Initialize LineRenderer properties
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Ensure there's a material assigned
        }

        // Set the initial rotation of the barrel
        if (barrel != null)
        {
            barrel.localRotation = Quaternion.Euler(barrelInitialRotation);
        }
    }

    void Update()
    {
        if (RTSManager.controllingTank)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;

            // Convert mouse position to a ray
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, maxRaycastDistance))
            {
                Vector3 targetPoint = hit.point;

                // Calculate the direction from the turret to the target point
                Vector3 direction = (targetPoint - turret.position).normalized;
                Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);
                Quaternion lookRotation = Quaternion.LookRotation(flatDirection);

                // Apply the initial rotation offset to the turret
                Quaternion turretOffsetRotation = Quaternion.Euler(turretRotationOffset);
                turret.rotation = Quaternion.Slerp(turret.rotation, lookRotation * turretOffsetRotation, rotationSpeed * Time.deltaTime);

                // Calculate the direction from the barrel to the target point
                Vector3 barrelToTarget = targetPoint - barrel.position;

                // Calculate the angle needed for the x-axis
                float angle = Mathf.Atan2(barrelToTarget.y, barrelToTarget.magnitude) * Mathf.Rad2Deg;

                // Create a rotation for the barrel that only affects the x-axis
                Quaternion barrelRotation = Quaternion.Euler(angle, barrel.localRotation.eulerAngles.y, barrel.localRotation.eulerAngles.z);

                // Smoothly apply the rotation
                barrel.localRotation = Quaternion.Slerp(barrel.localRotation, barrelRotation, rotationSpeed * Time.deltaTime);

                // Update LineRenderer positions
                if (lineRenderer != null)
                {
                    lineRenderer.SetPosition(0, lineStart.position);
                    lineRenderer.SetPosition(1, targetPoint);
                }

                // Move the targetObject to the target point
                if (targetObject != null)
                {
                    targetObject.position = targetPoint;
                }
            }
        }
    }
}
