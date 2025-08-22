using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_TroopControl : MonoBehaviour
{
    public Animator a;
    public KeyCode doAnimKey;
    public GameObject shootParticle;
    public Transform barrel;

    public int animState = 0; // 0 = not presented, 1 = ready to fire
    public bool isAnimating = false;

    private O_FactionControl brain;

    [SerializeField] private float bootHeight;

    public float hasReloadedTime = 0f;

    public bool isDead = false;
    void Start()
    {
        if (GetComponent<Animator>() != null)
            a = GetComponent<Animator>();
        brain = GetComponentInParent<O_FactionControl>();
    }

    void FixedUpdate()
    {
        //make sure boots are on ground        
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10))
        {
            //Debug.DrawLine(new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), hit.point, Color.red); // Visual debug
            //Debug.Log("Hit: " + hit.collider.name + " at distance: " + hit.distance);
            bootHeight = hit.point.y;
            transform.position = new Vector3(transform.position.x, bootHeight, transform.position.z);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.down * 10, Color.blue);
            Debug.Log("No ground hit detected.");
        }        

        /*if (Input.GetKeyDown(doAnimKey))
        {
            if (!brain.troopsInFaction.Contains(gameObject.GetComponent<O_TroopControl>()))
            {
                return;
            }
            if (animState == 0)
            {
                // FIRST PRESS: Random delay before presenting arms
                isAnimating = true;
                float delay = Random.Range(0f, 0.3f);
                StartCoroutine(DelayedPresent(delay));
            }
            else if (animState == 1 && Time.time >= hasReloadedTime)
            {
                isAnimating = true;
                float fireDelay = Random.Range(0f, 0.3f);
                StartCoroutine(DelayedFire(fireDelay));
            }
        }*/
    }

    public void PreparePresentOrFire()
    {
        if (!brain.troopsInFaction.Contains(gameObject.GetComponent<O_TroopControl>())) //prevent dead troops from firing
        {
            return;
        }
        if (animState == 0) //present arms! Do animation and wait for it to finish
        {
            isAnimating = true;
            float delay = Random.Range(0f, 0.3f);
            Invoke("DelayedPresent", delay);
        }
        else if (animState == 1 && Time.time >= hasReloadedTime) //FIRE! if we have reloaded
        {
            isAnimating = true;
            float fireDelay = Random.Range(0f, 0.3f);
            Invoke("DelayedFire", fireDelay);
        }
    }

    // function for delayed "Present Arms" with random delay
    public void DelayedPresent()
    {
        if (a == null) //stop doing anything if there is no animator
        {
            isAnimating = false;
            return;
        }
        a.Play("Troop_Present");

        //wait for animation to finish
        StartCoroutine(WaitForAnimation("Troop_Present", () =>
        {
            animState = 1; //set animState to 1 so we know we're firing next
            isAnimating = false;
        }));
    }
    // function for delayed "FIRE" with random delay
    public void DelayedFire()
    {
        bool fireDuds; //use this to fire non-lethal rounds if enemy formation is too weak (prevent conflicts and bugs)

        if (a == null)
        {
            isAnimating = false;
            return;
        }
        a.Play("Troop_FirenLoad");
        Instantiate(shootParticle, barrel.position, barrel.rotation);

        O_FormationControl myFormation = GetComponentInParent<O_FormationControl>();
        if (myFormation == null || myFormation.activeEnemy == null)
            return;

        O_FormationControl enemyFormation = myFormation.activeEnemy.GetComponent<O_FormationControl>();
        if (enemyFormation == null) //if active enemy has no formation control component
            fireDuds = true;

        // Get a list of LIVING troops
        List<O_TroopControl> validTargets = enemyFormation.troopsInFormation.FindAll(t => t != null && !t.isDead);
        if (validTargets.Count == 0) //if there are no valid troops in enemy formation
        {
            fireDuds = true;
        }
        else if (enemyFormation.killAllowance > 0)//if there are troops in validTargets and we have kills left in our allowance
        {
            // Pick a random living target
            O_TroopControl myTarget = validTargets[Random.Range(0, validTargets.Count)];

            if (myTarget != null)
            {
                enemyFormation.killAllowance--;

                //float distance = Vector3.Distance(transform.position, myTarget.transform.position);
                float accuracyChance = Mathf.Clamp(100 - (enemyFormation.troopsInFormation.Count * 0.4f), 5f, 95f); // Tunable
                float myRoll = Random.Range(0f, 100f);

                if (myRoll > accuracyChance)
                {                    
                    if (enemyFormation.troopsInFormation.Count > 5) //do not kill formations below 5 troops
                    {
                        float delay = Random.Range(0.01f, 0.1f);
                        int animId = Random.Range(0, 2);
                        //kill the enemy troop
                        enemyFormation.troopsInFormation.Remove(myTarget);
                        StartCoroutine(WaitAndKill(myTarget, delay, animId));
                    }
                }
            }
        }
        //wait for animation to finish
        StartCoroutine(WaitForAnimation("Troop_FirenLoad", () =>
        {
            isAnimating = false;
            hasReloadedTime = Time.time + 0.1f + Random.Range(0f, 0.3f); // adjustable
        }));
    }

    // Waits for an animation to finish, then calls the callback
    IEnumerator WaitForAnimation(string animationName, System.Action onComplete)
    {
        yield return null;

        while (!a.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            yield return null;

        float animLength = a.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        onComplete?.Invoke(); //isAnimating = false;
    }

    public static IEnumerator WaitAndKill(O_TroopControl t, float time, int animId)
    {
        yield return new WaitForSeconds(time);
        if (t != null && t.a != null)
        {
            t.isDead = true;
            if (animId == 0)
            {
                t.a.Play("Troop_Die");
                AudioHandler.Instance.PlayRandomSound("O_Scream0", "O_Scream1", "O_Scream2", null, true, true, t.transform.position, .8f, 0.3f);
            }
            else
            {
                t.a.Play("Troop_Die1");
                AudioHandler.Instance.PlayRandomSound("O_Scream3", "O_Scream4", "O_Scream5", "O_Scream6", true, true, t.transform.position, .8f, 0.45f);
            }
            t.transform.SetParent(null);
            //yield return new WaitForSeconds(t.a.GetCurrentAnimatorStateInfo(0).length);            
        }
    }
}
