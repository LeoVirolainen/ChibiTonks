using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_FactionControl : MonoBehaviour
{
    public List<O_TroopControl> troopsInFaction;
    public List<O_FormationControl> formations;
    //public KeyCode killKey;

    public int currentPhase;
    //0(A), 1(B), 2(B I), 3(C), 4(C I), 5(D), 6(D I)

    void Start()
    {
        troopsInFaction = new List<O_TroopControl>(GetComponentsInChildren<O_TroopControl>());
        formations = new List<O_FormationControl>(GetComponentsInChildren<O_FormationControl>());
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
            if (f.activeEnemy == null)
            {
                f.previousEnemy = null;
            }
            else
            {
                f.previousEnemy = f.activeEnemy;
            }
            if (f.myEnemies[currentPhase] == null)
            {
                f.activeEnemy = null;
            }
            else
            {
                f.activeEnemy = f.myEnemies[currentPhase];
            }
            f.WalkToGoal();
        }
    }
}
