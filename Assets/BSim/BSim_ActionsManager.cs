using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSim_ActionsManager : MonoBehaviour
{
    float nextEventTime;
    public float turnCooldown = 1;
    public List<BSim_Actor> allActors = new List<BSim_Actor>();
    // Start is called before the first frame update
    void Start()
    {
        BSim_Actor[] actors = FindObjectsOfType<BSim_Actor>();
        allActors = new List<BSim_Actor>(actors);
        nextEventTime = Time.time + turnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextEventTime)
        {
            RunAllActorEvents();
            nextEventTime = Time.time + turnCooldown;
        }        
    }

    void RunAllActorEvents()
    {
        foreach (BSim_Actor a in allActors)
        {
            a.DoAction();
        }
    }
}
