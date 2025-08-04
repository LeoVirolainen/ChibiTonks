using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_FormationControl : MonoBehaviour
{
    public AnimationCurve moveCurve;
    public KeyCode moveKey;

    public Transform currentMoveGoal;
    public Transform activeEnemy;
    private Vector3 startPos;
    private Vector3 goalPos;
    public List<O_TroopControl> troops;
    public Transform[] myMoveTargets = new Transform[7];
    public Transform[] myEnemies = new Transform[7];

    public Transform previousEnemy;

    private bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        troops = new List<O_TroopControl>(GetComponentsInChildren<O_TroopControl>());
    }

    // Update is called once per frame
    void Update()
    {
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
                //prepare or fire towards target
                StartCoroutine(FireVolley());
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
        StopAllCoroutines();
        startPos = transform.position;
        goalPos = new Vector3(currentMoveGoal.position.x, transform.position.y, currentMoveGoal.position.z);
        isMoving = true;
        StartCoroutine(LerpWithCurve());
    }
    public IEnumerator FireVolley()
    {
        var r = Random.Range(0, 4);
        if (r != 0)
        {
            yield return new WaitForSeconds(r / 2);
            foreach (O_TroopControl t in troops)
            {
                t.PreparePresentOrFire();
            }
        }
    }
    private IEnumerator LerpWithCurve()
    {
        float elapsed = 0f;
        float duration = Vector3.Distance(transform.position, currentMoveGoal.position) / 4;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveValue = moveCurve.Evaluate(t);
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
