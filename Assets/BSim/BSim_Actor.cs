using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BSim_Actor : MonoBehaviour
{
    public UnityEvent myNextAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoAction()
    {
        myNextAction.Invoke();
        //CalculateNextAction();
    }
    void CalculateNextAction()
    {
        bool moving = true;
        if (moving)
        {
            myNextAction.AddListener(DoMovementTowardsGoal);
        }
    }
    public void DoMovementTowardsGoal()
    {
        var pf = GetComponent<BSim_HexPathfind>();
        if (pf.goalReached) //if there is no existing path
            pf.CalculateNewPath();
        else
            pf.TakeNextStepOnPath();
    }
}
