using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI; // Add this to use NavMeshAgent

public class EnemySoldier : MonoBehaviour
{
    public List<Transform> playerObjects = new List<Transform>();
    public List<Transform> ammoBoxes = new List<Transform>();
    public List<Transform> coverObjects = new List<Transform>();
    //public float moveSpeed = 5f;
    public float stoppingDistance = 2f;
    //public float groundCheckDistance = 1f;
    //public float targetDistFromGround = 0.5f; // Height above the ground to maintain    

    public Transform currentTarget;
    private ArmorDestroyer shootScript;
    private NavMeshAgent agent; // Add a NavMeshAgent reference

    public float morale = 80f; // Starting morale value
    public float moraleThreshold = 40f; // Morale threshold for running mode
    public bool isRunning = false; // Running mode flag

    public TextMeshProUGUI moraleTxt;

    public enum TargetType
    {
        Player,
        Ammo,
        Cover
    }

    void Start()
    {
        moraleTxt = GetComponentInChildren<TextMeshProUGUI>();

        shootScript = GetComponent<ArmorDestroyer>();
        agent = GetComponent<NavMeshAgent>(); // Initialize the NavMeshAgent

        // Initialize player objects list
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerObjects.Add(target.transform);
        }
    }

    void Update()
    {
        // Calculate morale
        if (Vector3.Distance(transform.position, FindObjectOfType<TankMove>().transform.position) < 20)
        {
            morale -= Time.deltaTime * 2;
        }
        else
        {
            morale += Time.deltaTime;
        }
        moraleTxt.text = "Morale: " + morale.ToString();

        // Check morale and handle running mode
        if (morale < moraleThreshold)
        {
            isRunning = true;

            agent.speed = 4.5f;

            coverObjects.Clear();
            foreach (InfantryCover cover in FindObjectsOfType<InfantryCover>())
            {
                coverObjects.Add(cover.transform);
            }

            currentTarget = GetClosestTarget(TargetType.Cover);
        }
        else
        {
            isRunning = false;

            agent.speed = 3.5f;

            // If we have ammo and morale, find a player for target
            if (playerObjects.Count > 0)
            {
                currentTarget = GetClosestTarget(TargetType.Player);
            }
        }

        // Check ammo and find ammo boxes if necessary
        if (shootScript.ammo < 1 && !isRunning)
        {
            ammoBoxes.Clear();
            foreach (AmmoCrate box in FindObjectsOfType<AmmoCrate>())
            {
                ammoBoxes.Add(box.transform);
            }
            if (ammoBoxes.Count > 0)
                currentTarget = GetClosestTarget(TargetType.Ammo);
            else
                morale = 0;
        }

        // Set the agent's destination
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
            print(agent.destination);
            // Check if the agent is within the stopping distance
            if (currentTarget.GetComponent<TankMove>() != null && Vector3.Distance(transform.position, currentTarget.position) <= stoppingDistance)
            {
                agent.isStopped = true; // Stop the agent when within stopping distance
                print("STOP!");
            }
            else
            {
                agent.isStopped = false; // Resume movement
                print("let's go!");
            }
        }
    }



    public Transform GetClosestTarget(TargetType targetType)
    {
        List<Transform> targets = null;

        switch (targetType)
        {
            case TargetType.Player:
                targets = playerObjects;
                break;
            case TargetType.Ammo:
                targets = ammoBoxes;
                break;
            case TargetType.Cover:
                targets = coverObjects;
                break;
        }

        if (targets == null || targets.Count == 0) return null;

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target;
            }
        }

        return closest;
    }

    public void Die()
    {
        List<EnemySoldier> nearbySoldiers = new List<EnemySoldier>();

        foreach (EnemySoldier soldier in FindObjectsOfType<EnemySoldier>())
        {
            // Calculate the distance from this object to the soldier
            float distance = Vector3.Distance(transform.position, soldier.transform.position);

            if (distance <= 20)
            {
                nearbySoldiers.Add(soldier);
            }
        }

        foreach (EnemySoldier comrade in nearbySoldiers)
        {
            comrade.morale -= 100;
        }
        Destroy(gameObject);
    }
}
