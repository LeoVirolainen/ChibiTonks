using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BSim_Actor : MonoBehaviour
{    
    public enum ActorType
    {
        Troop,
        City
    }
    public ActorType type;

    public UnityEvent myNextAction;
    public bool isBlue;
    public List<BSim_HexPathfind> nearbyEnemies = new List<BSim_HexPathfind>();
    // Start is called before the first frame update
    void Start()
    {
        CalculateNextAction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoAction()
    {
        myNextAction.Invoke();        
        myNextAction.RemoveAllListeners();
        CalculateNextAction();
    }
    void CalculateNextAction()
    {
        if (type == ActorType.Troop)
        {
            if (nearbyEnemies.Count > 0)
            {
                int rand = Random.Range(0, nearbyEnemies.Count - 1);
                var target = nearbyEnemies[rand];
                print("Found " + nearbyEnemies[rand].name + "! Attacking now.");
            }
        }
        bool moving = true;
        if (moving)
        {
            myNextAction.AddListener(DoMovementTowardsGoal);
        }
    }
    public void DoMovementTowardsGoal()
    {
        var pf = GetComponent<BSim_HexPathfind>();
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
}
