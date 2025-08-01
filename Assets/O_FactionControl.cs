using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_FactionControl : MonoBehaviour
{
    public List<O_TroopControl> troops;
    public List<O_FormationControl> formations;
    public KeyCode killKey;

    public int currentPhase;
    //0(A), 1(B), 2(B I), 3(C), 4(C I), 5(D), 6(D I)

    void Start()
    {
        troops = new List<O_TroopControl>(GetComponentsInChildren<O_TroopControl>());
        formations = new List<O_FormationControl>(GetComponentsInChildren<O_FormationControl>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(killKey) && troops.Count > 0)
        {
            int numberToKill = Random.Range(1, Mathf.Min(6, troops.Count + 1)); // Between 1 and up to 5 or troop count, whichever is lower

            List<O_TroopControl> theDying = new List<O_TroopControl>();

            // Get distinct random indexes
            HashSet<int> chosenIndexes = new HashSet<int>();
            while (chosenIndexes.Count < numberToKill)
            {
                chosenIndexes.Add(Random.Range(0, troops.Count));
            }

            foreach (int index in chosenIndexes)
            {
                theDying.Add(troops[index]);
            }

            foreach (O_TroopControl troop in theDying)
            {
                troops.Remove(troop);

                float delay = Random.Range(0.01f, 0.1f); // stagger time
                int animId = Random.Range(0, 2); // 0 or 1
                StartCoroutine(WaitAndAnimate(troop, delay, animId));
            }
        }
    }
    //helper function for triggering and delaying troop death
    IEnumerator WaitAndAnimate(O_TroopControl t, float time, int animId)
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
    public void NextPhase()
    {
        if (currentPhase < 6)
        {
            currentPhase++;
            DoFormationMovements();
            SetActiveEnemies();
        }
        else
        {
            print("Reached final phase!");
        }
    }
    public void PrevPhase()
    {
        if (currentPhase > 0)
        {
            currentPhase--;
            DoFormationMovements();
            SetActiveEnemies();
        }
        else
        {
            print("Can't rewind further!");
        }
    }
    public void DoFormationMovements()
    {
        foreach (var f in formations)
        {
            if (f.myMoveTargets[currentPhase] != null)
            {
                f.currentMoveGoal = f.myMoveTargets[currentPhase];
                f.WalkToGoal();
            }
        }
    }
    public void SetActiveEnemies()
    {
        foreach (var f in formations)
        {
            if (f.myEnemies[currentPhase] != null)
            {
                f.activeEnemy = f.myEnemies[currentPhase];
                f.WalkToGoal();
            }
        }
    }
}
