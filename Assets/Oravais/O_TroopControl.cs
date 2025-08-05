using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_TroopControl : MonoBehaviour
{
    public Animator a;
    public KeyCode doAnimKey;
    public GameObject shootParticle;
    public Transform barrel;

    private float myRandTimer;
    public int animState = 0; // 0 = not presented, 1 = ready to fire
    [HideInInspector]public bool isAnimating = false;
    private float hasReloadedTime = 0f;

    private O_FactionControl brain;

    [SerializeField] private float bootHeight;

    void Start()
    {
        if (GetComponent<Animator>() != null)
            a = GetComponent<Animator>();
        brain = GetComponentInParent<O_FactionControl>();
        myRandTimer = Random.Range(0.0f, 0.3f);
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
        /*if (!brain.troopsInFaction.Contains(gameObject.GetComponent<O_TroopControl>()))
        {
            return;
        }*/
        if (animState == 0)
        {
            // FIRST PRESS: Random delay before presenting arms
            isAnimating = true;
            float delay = Random.Range(0f, 0.3f);
            Invoke("DelayedPresent", delay);
        }
        else if (animState == 1 && Time.time >= hasReloadedTime)
        {
            isAnimating = true;
            float fireDelay = Random.Range(0f, 0.3f);
            Invoke("DelayedFire", fireDelay);
        }
    }

    // Coroutine for delayed "Present Arms" with random delay
    public void DelayedPresent()
    {
        if (a == null)
        {
            return;
        }
        a.Play("Troop_Present");
        StartCoroutine(WaitForAnimation("Troop_Present", () =>
        {
            animState = 1;
            isAnimating = false;
        }));
    }
    public void DelayedFire()
    {
        if (a == null)
        {
            return;
        }
        a.Play("Troop_FirenLoad");
        Instantiate(shootParticle, barrel.position, barrel.rotation);
        hasReloadedTime = Time.time + 6f + Random.Range(0f, 0.3f); // new reload delay

        StartCoroutine(WaitForAnimation("Troop_FirenLoad", () =>
        {
            isAnimating = false;
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

        onComplete?.Invoke(); //isAnimating = false;, I guess?
    }
}
