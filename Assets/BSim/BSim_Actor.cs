using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using TMPro;

public class BSim_Actor : MonoBehaviour
{
    private BSim_HexPathfind pf;
    public enum ActorType
    {
        Troop,
        City
    }
    public ActorType type;
    public int manpower;
    private int startManpower;

    public UnityEvent myNextAction;
    public bool isBlue;
    public List<BSim_HexPathfind> nearbyEnemies = new List<BSim_HexPathfind>();

    public TextMeshProUGUI manpowerTxt;
    void Start()
    {
        pf = GetComponent<BSim_HexPathfind>();
        CalculateNextAction();
        startManpower = manpower;
        manpowerTxt = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        //draw line to move target if there is one
        if (pf.myPath.Count != 0)
        {
            Color color = GetComponent<SpriteRenderer>().color;
            // Convert Vector3Int to Vector3 for flat-top hex grid
            Tilemap tm = GetComponent<BSim_HexMove>().tilemap;
            Vector3 goalPosition = tm.CellToWorld(pf.goal);
            Debug.DrawLine(transform.position, goalPosition, color);
        }
    }

    public void DoAction()
    {
        myNextAction.Invoke();
        myNextAction.RemoveAllListeners();
        CalculateNextAction();
    }
    void CalculateNextAction()
    {
        ScanForEnemies();

        if (type != ActorType.Troop)
            return;

        if (nearbyEnemies.Count > 0)
        {
            int rand = Random.Range(0, nearbyEnemies.Count);
            var actor = nearbyEnemies[rand].GetComponent<BSim_Actor>();
            if (actor)
            {
                myNextAction.AddListener(() => DoAttackTowardsTarget(actor));
                return;
            }
        }

        myNextAction.AddListener(DoMovementTowardsGoal);
    }
    public void DoMovementTowardsGoal()
    {
        if (pf.myPath.Count == 0)
        {
            pf.CalculateNewPath();

            // Still no path? Fallback.
            if (pf.myPath.Count == 0)
            {
                print($"{name} couldn't find a path. Recalculating...");
                CalculateNextAction();
            }
        }
        else
        {
            pf.TakeNextStepOnPath();
        }
    }
    public void DoAttackTowardsTarget(BSim_Actor target)
    {
        int damage = (int)(manpower / 10);
        //make sure damage is at minimum 10 before randomizing
        if (damage < 50)
            damage = 50;
        damage *= (int)Random.Range(0.6f, 1.4f);
        if (damage <= 0)
            damage = 0;

        if (target == null)
        {
            print($"{name} tried to attack a null target!");
            CalculateNextAction(); // Retry
            return;
        }
        target.manpower -= damage;

        //change enemy manpower text
        target.manpowerTxt.text = target.manpower.ToString();

        if (target.manpower <= 0)
        {
            Destroy(target.gameObject);
            nearbyEnemies.Remove(target.GetComponent<BSim_HexPathfind>());
        }
    }
    void ScanForEnemies()
    {
        nearbyEnemies.Clear();

        // Get all 6 directions around you, assuming axial or offset coordinates
        Vector3Int myHexPos = GetComponent<BSim_HexMove>().currentHexPosition;
        Vector3Int[] directions = BSim_HexMove.GetNeighborOffsets(); // You'd make this helper

        foreach (var dir in directions)
        {
            Vector3Int neighborHex = myHexPos + dir;
            Vector3 worldPos = pf.tilemap.CellToWorld(neighborHex) + new Vector3(0.5f, 0.5f); // center of tile
            Debug.DrawRay(worldPos, Vector3.up);
            Collider[] hits = Physics.OverlapSphere(worldPos, 0.5f); // Adjust radius as needed
            Debug.DrawRay(worldPos, Vector3.up, Color.red, 1f);
            foreach (var hit in hits)
            {
                var actor = hit.GetComponent<BSim_Actor>();
                var pathfinder = hit.GetComponent<BSim_HexPathfind>();
                if (actor && pathfinder && actor.isBlue != isBlue)
                {
                    nearbyEnemies.Add(pathfinder);
                }
            }
        }
        Debug.Log($"Found {nearbyEnemies.Count} nearby enemies.");
    }
}
