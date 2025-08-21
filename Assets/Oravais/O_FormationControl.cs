using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
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

    public int killAllowance;
    float resetAllowanceTime;

    private Coroutine moveCoroutine;
    private bool isMoving = false;

    bool hasFired = false;
    // Start is called before the first frame update
    void Start()
    {
        troopsInFormation = new List<O_TroopControl>(GetComponentsInChildren<O_TroopControl>());
        resetAllowanceTime = Time.time + 6;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= resetAllowanceTime)
        {
            killAllowance = 3;
            resetAllowanceTime = Time.time + 6;
        }

        if (troopsInFormation.Count == 0)
        {
            Debug.Log($"{gameObject.name} has no troops left. Formation inactive.");
            return;
        }
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

        if (activeEnemy != null)
        {
            var targetRotation = Quaternion.LookRotation(new Vector3(activeEnemy.position.x, transform.position.y, activeEnemy.position.z) - transform.position);
            // Smoothly rotate towards the target point
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);

            //IF WE ARE FACING (CLOSE NOUGH) TARGET:                                              
            var dot = Vector3.Dot(viewDir, toOther);

            //if (gameObject.name == "1. Hälsingen Pataljoona")
            //print(gameObject.name + "'s DOT: " + dot);

            // find out if enough troops have reloaded to fire again
            int amtOfReadyTroops = 0;
            foreach (O_TroopControl t in troopsInFormation)
            {
                if (t.isAnimating == false)
                {
                    amtOfReadyTroops++;
                }
            }
            if (dot > 0.95f && amtOfReadyTroops > 0) //fire as soon as someone has reloaded
            {
                FireVolley();
                //print(gameObject.name + " Fire!");
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


    public void WalkToGoal()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        startPos = transform.position;
        goalPos = new Vector3(currentMoveGoal.position.x, transform.position.y, currentMoveGoal.position.z);
        isMoving = true;
        moveCoroutine = StartCoroutine(LerpWithCurve());
    }
    public void FireVolley()
    {
        if (hasFired == false)
        {
            List<O_TroopControl> firingTroops = new List<O_TroopControl>();
            foreach (O_TroopControl t in troopsInFormation)
            {
                if (!t.isAnimating && Time.time >= t.hasReloadedTime)
                {
                    t.PreparePresentOrFire(); //make this trooper shoot
                    firingTroops.Add(t); //keep count of how many troops are firing
                }
            }
            foreach (O_TroopControl shooter in firingTroops)
            {
                if (shooter.Equals(firingTroops[firingTroops.Count - 1])) //if this troop is the last one in the list
                {
                    //this is the last troop to fire! Play the sound.
                    if (firingTroops.Count > 10)
                    {
                        //play big volley sound
                        AudioHandler.Instance.PlayRandomSound("O_Volley0", "O_Volley1", "O_Volley2", null, true, true, transform.position, 1);
                    }
                    else if (firingTroops.Count > 0)
                    {
                        //play smol volley sound
                        AudioHandler.Instance.PlayRandomSound("O_Openfire0", "O_Openfire1", "O_Openfire2", "O_Openfire3", true, true, transform.position, 1);
                    }
                }
            }
            hasFired = true;
            StartCoroutine(WaitForFiring());
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
    private IEnumerator WaitForFiring()
    {
        float time = UnityEngine.Random.Range(0.4f, 1.2f);
        yield return new WaitForSeconds(time);
        hasFired = false;
    }
}
