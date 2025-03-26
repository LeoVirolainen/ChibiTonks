using System.Collections;
using UnityEngine;

public class SpiderLegController : MonoBehaviour
{
    public Transform[] legTargets;  // Array of leg target transforms
    public Transform body;          // Reference to the mecha body
    public float stepHeight = 0.2f; // How high the leg lifts during a step
    public float stepSpeed = 5f;    // Speed of the leg movement
    public float stepDistance = 1.5f; // Minimum distance to trigger a step
    public float maxLegDistance = 2.0f; // Maximum distance leg can reach from body before stepping

    private Vector3[] initialLegPositions; // Initial leg positions relative to the body
    private bool[] isStepping;  // Flags for whether a leg is currently stepping

    void Start()
    {
        initialLegPositions = new Vector3[legTargets.Length];
        isStepping = new bool[legTargets.Length];

        // Initialize leg positions relative to the body
        for (int i = 0; i < legTargets.Length; i++)
        {
            initialLegPositions[i] = legTargets[i].position - body.position;
        }
    }

    void Update()
    {
        MoveBody();
        UpdateLegTargets();
    }

    void MoveBody()
    {
        // Example body movement: moving forward continuously
        body.position += body.forward * Time.deltaTime * 1f;
    }

    void UpdateLegTargets()
    {
        for (int i = 0; i < legTargets.Length; i++)
        {
            if (!isStepping[i])
            {
                // Calculate the desired leg position relative to the body's current position and orientation
                Vector3 desiredPosition = body.position + body.TransformDirection(initialLegPositions[i]);

                // Calculate the distance from the current leg position to the desired position
                float distanceToDesiredPosition = Vector3.Distance(legTargets[i].position, desiredPosition);
                Debug.Log($"Leg {i} - Current Position: {legTargets[i].position}, Desired Position: {desiredPosition}, Distance: {distanceToDesiredPosition}");

                // Check if the leg is too far from the desired position
                if (distanceToDesiredPosition > stepDistance)
                {
                    Debug.Log($"Leg {i} starts stepping!");
                    StartCoroutine(MoveLeg(i, desiredPosition));
                }
            }
        }
    }

    IEnumerator MoveLeg(int legIndex, Vector3 targetPosition)
    {
        isStepping[legIndex] = true;
        Vector3 startPosition = legTargets[legIndex].position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * stepSpeed;
            float heightOffset = Mathf.Sin(t * Mathf.PI) * stepHeight;
            Vector3 nextPosition = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * heightOffset;

            // Adjust height with a raycast to the ground
            if (Physics.Raycast(nextPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                nextPosition.y = hit.point.y + 0.1f; // Ensures the leg is slightly above the ground
            }

            legTargets[legIndex].position = nextPosition;

            yield return null;
        }

        legTargets[legIndex].position = targetPosition;
        isStepping[legIndex] = false;
    }
}
