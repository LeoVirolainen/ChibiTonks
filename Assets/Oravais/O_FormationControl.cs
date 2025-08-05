using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class O_FormationControl : MonoBehaviour
{
    public AnimationCurve moveCurve;
    public KeyCode moveKey;

    public Transform currentMoveGoal;
    public Transform activeEnemy;
    private Vector3 startPos;
    private Vector3 goalPos;
    public List<O_TroopControl> troopsInFormation;
    public Transform[] myMoveTargets = new Transform[7];
    public Transform[] myEnemies = new Transform[7];

    public Transform previousEnemy;
    Vector3 viewDir;
    Vector3 toOther;

    private Coroutine moveCoroutine;
    private bool isMoving = false;

    public bool hasFired = false;
    // Start is called before the first frame update
    void Start()
    {
        troopsInFormation = new List<O_TroopControl>(GetComponentsInChildren<O_TroopControl>());
    }

    // Update is called once per frame
    void Update()
    {
        //get forward vector in world space
        viewDir = transform.TransformDirection(Vector3.forward);
        //get vector pointing to target
        if (activeEnemy != null)
        {
            toOther = Vector3.Normalize(activeEnemy.position - transform.position);
            Debug.DrawRay(transform.position, toOther * Vector3.Distance(transform.position, activeEnemy.position), Color.red);
        }
        Debug.DrawRay(transform.position, viewDir, Color.green);


        if (Input.GetKeyDown(moveKey))
        {
            WalkToGoal();
        }
        if (isMoving == false)
        {
            if (activeEnemy != null)
            {
                var targetRotation = Quaternion.LookRotation(new Vector3(activeEnemy.position.x, transform.position.y, activeEnemy.position.z) - transform.position);
                // Smoothly rotate towards the target point
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);

                //IF WE ARE FACING (CLOSE NOUGH) TARGET:                                              
                var dot = Vector3.Dot(viewDir, toOther);

                //if (gameObject.name == "1. Hälsingen Pataljoona")
                    //print(gameObject.name + "'s DOT: " + dot);

                if (dot > 0.95f && !hasFired)
                {
                    hasFired = true;
                    StartCoroutine(FireVolley());
                }
            }
            else
            {
                if (previousEnemy != null)
                {
                    var targetRotation = Quaternion.LookRotation(new Vector3(previousEnemy.position.x, transform.position.y, previousEnemy.position.z) - transform.position);
                    // Smoothly rotate towards the target point
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);
                }
            }
        }
    }

    public void WalkToGoal()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        startPos = transform.position;
        goalPos = new Vector3(currentMoveGoal.position.x, transform.position.y, currentMoveGoal.position.z);
        isMoving = true;
        moveCoroutine = StartCoroutine(LerpWithCurve());
    }
    public IEnumerator FireVolley()
    {
        foreach (O_TroopControl t in troopsInFormation)
        {
            if (!t.isAnimating)
            {
                t.PreparePresentOrFire();
            }
        }        
        //check if active enemy is null, if so, STOP IMMEDIATELY.
        if (activeEnemy == null) yield return null;
        if (!activeEnemy.gameObject.activeInHierarchy) yield return null;
        var enemyFormation = activeEnemy.GetComponent<O_FormationControl>();
        if (enemyFormation != null)
        {
            //enemyFormation.TakeCasualties(troopsInFormation.Count);
        }

        if (troopsInFormation[0].animState == 0)
        {
            yield return new WaitForSeconds(1.1f); //wait for presenting arms
        }
        else
        {
            yield return new WaitForSeconds(6); //wait for reload
        }
        hasFired = false;
    }
    public void TakeCasualties(int attackerStrength)
    {
        if (troopsInFormation.Count > 0)
        {
            int numberToKill = Random.Range(1, Mathf.Min(6, attackerStrength)); // Between 1 and 6 or enemy troop count, whichever is lower

            List<O_TroopControl> theDying = new List<O_TroopControl>();

            // Get distinct random indexes
            HashSet<int> chosenIndexes = new HashSet<int>();
            while (chosenIndexes.Count < numberToKill)
            {
                chosenIndexes.Add(Random.Range(0, troopsInFormation.Count));
            }

            // add each troop of corresponding index to new list of killable troops
            foreach (int index in chosenIndexes)
            {
                theDying.Add(troopsInFormation[index]);
            }

            foreach (O_TroopControl troop in theDying)
            {                
                troopsInFormation.Remove(troop);

                float delay = Random.Range(0.01f, 0.1f); // stagger time
                int animId = Random.Range(0, 2); // dying animation v.0 or v.1
                StartCoroutine(WaitAndAnimate(troop, delay, animId));
            }
        }
    }
    //helper function for triggering and delaying troop death
    public static IEnumerator WaitAndAnimate(O_TroopControl t, float time, int animId)
    {
        yield return new WaitForSeconds(time);
        if (t != null && t.a != null)
        {
            if (animId == 0)
                t.a.Play("Troop_Die");
            else
                t.a.Play("Troop_Die1");
            t.transform.SetParent(null);
        }
    }
    private IEnumerator LerpWithCurve()
    {
        float elapsed = 0f;
        float duration = Vector3.Distance(transform.position, currentMoveGoal.position) / 4;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveValue = Mathf.Clamp01(moveCurve.Evaluate(t));
            transform.position = Vector3.Lerp(startPos, goalPos, curveValue);

            var targetRotation = Quaternion.LookRotation(goalPos - transform.position);
            // Smoothly rotate towards the target point
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, duration * 2 * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }
        isMoving = false;
        transform.position = goalPos; // Snap just to be safe
    }
}
