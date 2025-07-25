using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_FormationControl : MonoBehaviour
{
    public AnimationCurve moveCurve;
    public KeyCode moveKey;

    public Transform goal;
    public Transform activeEnemy;
    private Vector3 startPos;
    private Vector3 goalPos;
    public List<O_TroopControl> troops;

    private Vector3 velocity = Vector3.zero; // Internal velocity tracker

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
            var targetRotation = Quaternion.LookRotation(new Vector3(activeEnemy.position.x, transform.position.y, activeEnemy.position.z) - transform.position);
            // Smoothly rotate towards the target point
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);
        }
    }

    public void WalkToGoal()
    {
        StopAllCoroutines();
        startPos = transform.position;
        goalPos = new Vector3(goal.position.x, transform.position.y, goal.position.z);
        isMoving = true;
        StartCoroutine(LerpWithCurve());
    }
    public void FireVolley()
    {
        foreach(O_TroopControl t in troops)
        {
            t.PresentOrFire();
        }
    }
    private IEnumerator LerpWithCurve()
    {
        float elapsed = 0f;
        float duration = Vector3.Distance(transform.position, goal.position) / 4;

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
