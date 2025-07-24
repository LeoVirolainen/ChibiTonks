using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_FactionControl : MonoBehaviour
{
    public List<O_TroopControl> troops;
    public KeyCode killKey;
    // Start is called before the first frame update
    void Start()
    {
        troops = new List<O_TroopControl>(GetComponentsInChildren<O_TroopControl>());

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
    IEnumerator WaitAndAnimate(O_TroopControl t, float time, int animId)
    {
        yield return new WaitForSeconds(time);
        if (t != null && t.a != null)
        {
            if (animId == 0)
                t.a.Play("Troop_Die");
            else
                t.a.Play("Troop_Die1");
        }
    }
}
