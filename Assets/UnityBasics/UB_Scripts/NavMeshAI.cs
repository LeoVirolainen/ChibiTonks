using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAI : MonoBehaviour
{
    /*THINKS 
     What should this do?
    - enum: allow variants
        - follower (follows thing x)
        - wanderer (wanders randomly around x radius)
        - guard (looks for target, if sees it, moves to it, if target missing for x time, returns to post)
            - knows target -> cast raycast toward it -> if hits player = visible -> set plr as dest
            - if ray doesn't hit player for x time -> set origin as dest
        - hunter (wanders randomly, if sees target, fires at it, if stops seeing target, 
          moves to last seen location, continues wandering after x time.
            - check if seeing player -> yes? fire at! + save plr location
            -> no? -> has saved plr location? -> yes: move to where we last saw plr -> wait and keep scanning -> continue wandering after time
                                                -> no: wander
     */
    
    public enum AIType {Follower, Wanderer, Guard, Hunter}
    public AIType behaviour;

    private NavMeshAgent agent;
    public Transform target;

    [Tooltip("How far should I move from my starting point? 0 = infinite")]
    public float moveRange = 10;
    private Vector3 startPos;

    [Tooltip("How close to my destination should I move?")]
    public float stopDist;

    [Tooltip("How far can I see? Needed only for guard/hunter behaviours.")]
    public float sightRange = 20;

    [Tooltip("(Guard) How long do I wait before going back to my post after losing sight of my target?")]
    public float searchTime;
    [SerializeField] private bool hasGivenUpSearch;

    [SerializeField]private float goToStartT;

    [SerializeField] private bool isSettingDest = false;

    [SerializeField] private bool isWandering = false;
    [SerializeField] private Vector3 savedTargetPos;
    void Start()
    {
        startPos = transform.position;
        agent = GetComponent<NavMeshAgent>();
        HandleType(behaviour);       
    }

    void Update()
    {

    }

    void HandleType(AIType type)
    {
        switch (type)
        {
            case AIType.Follower:
                    InvokeRepeating("SetDest", 0.1f, 0.3f);
                break;
            case AIType.Wanderer:
                SetRandomDest();
                InvokeRepeating("CheckWander", 0.1f, 0.3f);
                break;
            case AIType.Guard:
                InvokeRepeating("LookForTarget", 0.1f, 0.3f);
                break;
            case AIType.Hunter:
                InvokeRepeating("HuntForTarget", .1f, .3f);
                InvokeRepeating("CheckWander", .2f, .3f);
                break;
            default:
                //go to default action
                break;
        }
    }
    //Follower logic
    void SetDest()
    {
        if (Vector3.Distance(startPos, target.position) < moveRange || moveRange == 0)
        {
            if (Vector3.Distance(transform.position, target.position) > stopDist)
                agent.destination = target.position;
        }
    }
    //Wanderer logic
    void CheckWander()
    {
        if (behaviour == AIType.Hunter)
        {
            if (isWandering == true)
            {
                if (Vector3.Distance(transform.position, agent.destination) < stopDist && !isSettingDest)
                {
                    SetRandomDest();
                }
            }
        }
        else if (Vector3.Distance(transform.position, agent.destination) < stopDist && !isSettingDest)
        {
            SetRandomDest();
        }        
    }
    void SetRandomDest()
    {
        isSettingDest = true;  // Prevent multiple calls
        if (moveRange == 0 || behaviour == AIType.Hunter)
        {
            agent.destination = transform.position + Random.insideUnitSphere * 5 * 2;
        }
        else
        {
            agent.destination = startPos + Random.insideUnitSphere * moveRange * 2;
        }
        isSettingDest = false;  // Allow new calls once movement starts
    }
    //Guard logic
    void LookForTarget()
    {
        // Define a ray towards target and cast it
        Vector3 rayDir = (target.position - transform.position).normalized;       
        if (Physics.Raycast(transform.position, rayDir, out RaycastHit hit, sightRange))
        {
            // Get the parent of hit object and check if it's our target
            Transform hitObj = GetHighestParent(hit.collider.transform);
            if (hitObj == target)
            {
                //print("I can see player!");
                // Target is visible!
                SetDest();
                goToStartT = Time.time + searchTime;
                hasGivenUpSearch = false;
            }          
        }
        if (Time.time > goToStartT && hasGivenUpSearch == false)
        {
            hasGivenUpSearch = true;
            agent.destination = startPos;         
        }
    }
    //Hunter logic
    void HuntForTarget()
    {
        Vector3 rayDir = (target.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, rayDir, out RaycastHit hit, sightRange))
        {
            Transform hitObj = GetHighestParent(hit.collider.transform);

            // PRIORITY 1: Target is visible - attack and save target pos
            if (hitObj == target)
            {
                hasGivenUpSearch = false;
                isWandering = false;
                //print("Boom! Die " + target.name + "!");
                agent.isStopped = true;
                savedTargetPos = target.position;
            }
            // PRIORITY 2: Target not visible - move to saved target pos
            else if (hitObj != target)
            {
                //print("Can't see my target.");
                HandleMissedHuntScan();
            }
        }
        // PRIORITY 3: Raycast missed completely - move to saved target pos
        else
        {
            //print("I see NOTHING!");
            HandleMissedHuntScan();
        }        
    }
    void HandleMissedHuntScan()
    {
        // (PRIORITY 2+3) has saved pos and hasn't gone there yet - go there.
        if (savedTargetPos != Vector3.zero && Vector3.Distance(transform.position, savedTargetPos) > stopDist)
        {
            //print("I saw the player at " + savedTargetPos.ToString() + ", moving there now.");
            agent.isStopped = false;
            agent.destination = savedTargetPos;
            goToStartT = Time.time + searchTime;
            hasGivenUpSearch = false;
            return;
        }
        // PRIORITY 4: has saved pos but reached it - scan for player before giving up
        if (savedTargetPos != Vector3.zero && Vector3.Distance(transform.position, savedTargetPos) < stopDist && !hasGivenUpSearch)
        {
            //print("checking around last known location...");
            if (Time.time > goToStartT)
            {                
                if (!isWandering)
                {
                    //print("Time's up, wandering now.");
                    StartWandering();
                }                    
            }
        }
        // (PRIORITY 2+3) if has no saved pos
        else
        {            
            if (!isWandering)
            {
                //print("never saw target OR already scanned saved pos. Wandering.");
                StartWandering();
            }                
        }
    }
    void StartWandering()
    {
        //print("time is: " + Time.time);
        hasGivenUpSearch = true;
        savedTargetPos = Vector3.zero;
        agent.isStopped = false;
        SetRandomDest();
        isWandering = true;
        return;
    }
    Transform GetHighestParent(Transform obj)
    {
        while (obj.parent != null)
        {
            obj = obj.parent;
        }
        return obj;
    }
}
