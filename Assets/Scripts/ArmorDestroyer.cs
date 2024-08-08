using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorDestroyer : MonoBehaviour
{
    //This script looks for target thanks and throws grenades/shoots rockets at them
    public bool lookForPlayerTank = true;
    public GameObject projectile;
    public Transform projectileStart;
    public Transform currentTarget;
    public int ammo = 3;
    public float reloadTime = 3f;
    float randomness = 0.1f;
    float timeOfNextShot;

    float lookForTargetInterval = 5f;
    float timeOfNextTargetSearch;

    public bool isSoldier = true;
    EnemySoldier soldierScript;

    void Start()
    {
        if (isSoldier)
        {
            soldierScript = GetComponent<EnemySoldier>();
        }

        FindNearestTank(); // look for tanks
        timeOfNextTargetSearch = Time.time + lookForTargetInterval; //calculate time of second tank search


    }

    void Update()
    {
        if (Time.time > timeOfNextTargetSearch || currentTarget == null)
        { //routinely look for targets or find one if has none
            FindNearestTank();
            timeOfNextTargetSearch = Time.time + lookForTargetInterval;
        }
        if (ammo > 0)
        {
            if (Time.time > timeOfNextShot)
            {
                if (isSoldier)
                {
                    if (!soldierScript.isRunning)
                    {
                        FireAtTank();
                        timeOfNextShot = Time.time + reloadTime;
                    }
                }
                else
                {
                    FireAtTank();
                    timeOfNextShot = Time.time + reloadTime;
                }                
            }
        }
    }

    void FindNearestTank() //if set to look for player tanks, find the closest one and set as target
    {
        if (lookForPlayerTank)
        {
            if (!isSoldier)
            { //if this is not a soldier and doesn't have a target finder method in another script
                var targets = FindObjectsOfType<TankMove>();
                foreach (TankMove target in targets)
                {
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    float smallestDist = Mathf.Infinity;
                    if (distance < smallestDist)
                    {
                        smallestDist = distance;
                        currentTarget = target.transform;
                    }
                }
            }
            else
            {
                currentTarget = soldierScript.GetClosestTarget(EnemySoldier.TargetType.Player);
            }
        }
    }

    void FireAtTank()
    {
        if (currentTarget != null && currentTarget.gameObject.CompareTag("Player"))
        {
            // Calculate the distance to the target
            Vector3 toTarget = currentTarget.position - projectileStart.position;
            float distance = toTarget.magnitude;

            // Calculate the required launch velocity using the formula for projectile motion
            float launchAngle = 45f * Mathf.Deg2Rad; // Convert to radians
            float g = Physics.gravity.y; // Gravity (negative value)

            // Calculate the required initial velocity using the formula:
            // velocity = sqrt(distance * g / sin(2 * launchAngle))
            float launchVelocity = Mathf.Sqrt(distance * -g / Mathf.Sin(2 * launchAngle));

            // Calculate the launch direction at 45 degrees upwards
            Vector3 launchDirection = (toTarget.normalized + Vector3.up).normalized;

            // Add some randomness to the aim
            launchDirection.x += Random.Range(-randomness, randomness);
            launchDirection.z += Random.Range(-randomness, randomness);

            // Instantiate the projectile
            GameObject shot = Instantiate(projectile, projectileStart.position, Quaternion.identity);

            // Ensure the projectile has a Rigidbody component
            Rigidbody rb = shot.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Apply the calculated force to the projectile
                rb.AddForce(launchDirection * launchVelocity, ForceMode.VelocityChange);
            }

            //deduct ammo
            ammo--;
        }
        else
        {
            FindNearestTank();
        }
    }
}
