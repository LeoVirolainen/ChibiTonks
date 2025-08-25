using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class O_FactionControl : MonoBehaviour
{
    public List<O_TroopControl> troopsInFaction;
    public List<O_FormationControl> formations;
    public O_TextHandler textCode;

    public O_PlayheadHandler playhead;
    //public KeyCode killKey;

    public int currentPhase;
    public int maxPhase = 6;
    //0(A), 1(B), 2(B I), 3(C), 4(C I), 5(D), 6(D I)

    void Start()
    {
        troopsInFaction = new List<O_TroopControl>(GetComponentsInChildren<O_TroopControl>());
        formations = new List<O_FormationControl>(GetComponentsInChildren<O_FormationControl>());
        textCode = GetComponent<O_TextHandler>();
        if (textCode != null)
            textCode.OnPhaseChanged(currentPhase);
    }
    public void NextPhase()
    {
        if (currentPhase < maxPhase)
        {
            currentPhase++;
            ChangePhase();
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
            ChangePhase();
        }
        else
        {
            print("Can't rewind further!");
        }
    }
    void ChangePhase()
    {
        DoFormationMovements();
        SetActiveEnemies();
        if (playhead != null)
            playhead.MovePlayhead();
        if (textCode != null)
        {
            playhead.GetComponentInChildren<TextMeshProUGUI>().text = textCode.OnPhaseChanged(currentPhase);
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
