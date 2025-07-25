using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BSim_Actor : MonoBehaviour
{
    private BSim_HexPathfind pf;
    public enum ActorType
    {
        Troop,
        City
    }
    public ActorType type;

    public UnityEvent myNextAction;
    public bool isBlue;
    public List<BSim_HexPathfind> nearbyEnemies = new List<BSim_HexPathfind>();

    public float hexWidth = 1.0f; // Width of the hexagon
    public float hexHeight = 0.8659766f; // Height of the hexagon
    void Start()
    {
        pf = GetComponent<BSim_HexPathfind>();
        CalculateNextAction();
    }

    void Update()
    {
        //draw line to move target if there is one
        if (pf.myPath.Count != 0)
        {
            Color color = new Color(0, 0, 1.0f);
            // Convert Vector3Int to Vector3 for flat-top hex grid
            Vector3 goalPosition = HexToWorld(pf.goal);
            Debug.DrawLine(transform.position, goalPosition, color);            
        }
    }

    public void DoAction()
    {
        myNextAction.Invoke();        
        myNextAction.RemoveAllListeners();
        CalculateNextAction();
    }
    void CalculateNextAction()
    {
        bool moving = false;
        if (type == ActorType.Troop)
        {
            if (nearbyEnemies.Count > 0)
            {
                int rand = Random.Range(0, nearbyEnemies.Count - 1);
                var target = nearbyEnemies[rand];
                print("Found " + nearbyEnemies[rand].name + "! Attacking now.");

            }
            else
            {
                moving = true;
            }
        }
        if (moving)
        {
            myNextAction.AddListener(DoMovementTowardsGoal);
        }
    }
    public void DoMovementTowardsGoal()
    {
        if (pf.myPath.Count == 0) //if there is no existing path
            pf.CalculateNewPath();
        else
            pf.TakeNextStepOnPath();
    }

    private void OnTriggerEnter(Collider other)
    {
        var actor = other.gameObject.GetComponent<BSim_Actor>();
        var pathfind = other.gameObject.GetComponent<BSim_HexPathfind>();

        if (pathfind != null && actor != null && actor.isBlue != isBlue)
        {
            nearbyEnemies.Add(pathfind);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var actor = other.gameObject.GetComponent<BSim_Actor>();
        var pathfind = other.gameObject.GetComponent<BSim_HexPathfind>();

        if (pathfind != null && actor != null && actor.isBlue != isBlue)
        {
            nearbyEnemies.Remove(pathfind);
        }
    }
    Vector3 HexToWorld(Vector3Int hex)
    {
        // Calculate the world position based on hex coordinates
        float x = hexWidth * (hex.x + 0.5f * hex.y); // Adjust x based on y
        float z = hexHeight * hex.y; // Adjust z based on y

        return new Vector3(x, 0, z); // Assuming y is 0 for flat-top hexes
    }
}
