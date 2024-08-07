using System.Collections.Generic;
using UnityEngine;

public class EnemySoldier : MonoBehaviour
{
    public List<Transform> playerObjects = new List<Transform>();
    public float moveSpeed = 5f;
    public float stoppingDistance = 2f;
    public float groundCheckDistance = 1f;
    public float targetDistFromGround = 0.5f; // Height above the ground to maintain

    private Transform closestPlayer;

    void Start()
    {
        // Fill the playerObjects list with all objects tagged "target"
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject target in targets)
        {
            playerObjects.Add(target.transform);
        }
    }

    void Update()
    {
        if (playerObjects.Count == 0)
            return;

        closestPlayer = GetClosestPlayer();

        if (closestPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, closestPlayer.position);

            if (distanceToPlayer > stoppingDistance)
            {
                MoveTowardsPlayer();
            }
        }

        AdjustHeightFromGround();
    }

    Transform GetClosestPlayer()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform player in playerObjects)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player;
            }
        }

        return closest;
    }

    void MoveTowardsPlayer()
    {
        // Move towards the closest player on the horizontal plane only
        Vector3 direction = (closestPlayer.position - transform.position).normalized;
        direction.y = 0; // Prevent movement in the y-axis when moving horizontally
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void AdjustHeightFromGround()
    {
        RaycastHit hit;
        // Perform a raycast downwards to check the distance to the ground
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            float distanceToGround = hit.distance;
            // Adjust the height to maintain the specified distance from the ground
            if (Mathf.Abs(distanceToGround - targetDistFromGround) > 0.01f)
            {
                // Calculate the new position with the correct height
                float newYPosition = hit.point.y + targetDistFromGround;
                transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
            }
        }
    }
}
