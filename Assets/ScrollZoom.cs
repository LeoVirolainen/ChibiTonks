using UnityEngine;

public class ScrollZoom : MonoBehaviour
{
    public float zoomSpeed = 10f; // Speed of zooming
    public float minZoomDistance = 10f; // Minimum distance from terrain
    public float maxZoomDistance = 50f; // Maximum distance from terrain
    public LayerMask terrainLayer; // Layer mask to ensure we only hit terrain colliders
    public float smoothSpeed = 0.1f; // Speed of smoothing

    private Rigidbody rb;
    private float currentZoomDistance;

    private void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
        currentZoomDistance = Vector3.Distance(transform.position, GetTerrainHitPoint());
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomDirection = transform.forward * scroll * zoomSpeed * Time.deltaTime;
        print(scroll + " " + zoomDirection);

        rb.AddForce(zoomDirection, ForceMode.Force);
        currentZoomDistance = Vector3.Distance(transform.position, GetTerrainHitPoint());
        transform.localPosition = new Vector3(0, currentZoomDistance, 0);
    }

    private void FixedUpdate()
    {
        /*// Perform a raycast forward from the camera's position
        RaycastHit hit;
        Vector3 terrainHitPoint = GetTerrainHitPoint();
        float distanceToTerrain = Vector3.Distance(transform.position, terrainHitPoint);

        if (distanceToTerrain < minZoomDistance || distanceToTerrain > maxZoomDistance)
        {
            Vector3 clampedPosition = transform.position;

            if (distanceToTerrain < minZoomDistance)
            {
                clampedPosition = terrainHitPoint + transform.forward * (minZoomDistance - distanceToTerrain);
            }
            else if (distanceToTerrain > maxZoomDistance)
            {
                clampedPosition = terrainHitPoint + transform.forward * (maxZoomDistance - distanceToTerrain);
            }

            // Smoothly move to the clamped position
            if (rb != null)
            {
                rb.MovePosition(Vector3.Lerp(transform.position, clampedPosition, smoothSpeed));
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
            }
        }*/
    }

    private Vector3 GetTerrainHitPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxZoomDistance, terrainLayer))
        {
            return hit.point;
        }
        return transform.position; // Default to current position if no terrain is hit
    }
}
