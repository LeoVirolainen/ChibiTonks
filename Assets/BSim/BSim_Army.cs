using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BSim_Army : MonoBehaviour
{
    public int teamID; // 0 for Blue, 1 for Red

    public float health = 500000;
    public float moveSpeed;
    public float fightSpeed = 0.2f;

    public Transform target;
    public bool isFighting;
    public int enemiesInContact;

    public TextMeshProUGUI hpTxt;
    private Rigidbody rb;
    private List<BSim_Army> enemies = new List<BSim_Army>();
    private void Start()
    {
        SelectTarget();
        rb = GetComponent<Rigidbody>();

        if (hpTxt == null)
        {
            hpTxt = GetComponentInChildren<TextMeshProUGUI>();
        }
        hpTxt.text = health.ToString();

        InvokeRepeating("Fight", 0.1f, fightSpeed);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (enemies.Contains(collision.gameObject.GetComponent<BSim_Army>()))
        {
            enemiesInContact++;
            target = collision.transform;
            if (enemiesInContact > 0)
            {
                isFighting = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (enemies.Contains(collision.gameObject.GetComponent<BSim_Army>()))
        {
            enemiesInContact--;
            if (enemiesInContact <= 0)
            {
                isFighting = false;
            }
            SelectTarget();
        }
    }
    private void Fight()
    {
        if (isFighting && target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= 1.1f) // adjust based on your unit size
            {
                int strike = Mathf.RoundToInt(Random.Range(health * 0.01f, health * 0.1f));
                var enemy = target.GetComponent<BSim_Army>();
                enemy.health -= strike;
                enemy.hpTxt.text = enemy.health.ToString();

                if (enemy.health <= 0)
                {
                    Destroy(enemy.gameObject);
                    enemiesInContact = 0;
                    isFighting = false;
                    SelectTarget();
                }
            }
            else
            {
                // Out of reach, stop fighting
                isFighting = false;
            }
        }
        else if (target == null)
        {
            SelectTarget();
        }
    }

    private void Update()
    {
        if (!isFighting && target != null)
        {
            enemiesInContact = 0;

            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        else if (target == null && !isFighting)
        {
            // No enemies left — maybe idle animation, stop movement, or rotate slowly
            rb.velocity = Vector3.zero; // Stop drift just in case
        }
    }

    private void SelectTarget()
    {
        // Get all units in the scene
        BSim_Army[] allUnits = FindObjectsOfType<BSim_Army>();

        enemies.Clear();
        // Filter out self and same-team units        
        foreach (BSim_Army unit in allUnits)
        {
            if (unit != this && unit.teamID != this.teamID)
            {
                enemies.Add(unit);
            }
        }

        // Pick one at random, if any
        if (enemies.Count > 0)
        {
            int index = Random.Range(0, enemies.Count);
            target = enemies[index].transform;
        }
        if (enemies.Count == 0)
        {
            target = null;
            isFighting = false;
        }
    }
}
