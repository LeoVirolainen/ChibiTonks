using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour
{
    public int health = 14;
    private int maxHealth;
    public List<ArmorPlate> armorPlates = new List<ArmorPlate>();
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;

        // Get all components of type 'ComponentType' in the children of this GameObject.
        ArmorPlate[] componentsArray = GetComponentsInChildren<ArmorPlate>();

        // Add each component to the list.
        armorPlates.AddRange(componentsArray);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RepairDamage(int amount)
    {
        // Ensure the list is not empty to avoid errors.
        if (armorPlates.Count < maxHealth)
        {
            ArmorPlate[] allArmorPlates = GetComponentsInChildren<ArmorPlate>(true);

            int addedCount = 0;

            while (addedCount < amount && armorPlates.Count < maxHealth)
            {
                // Select a random ArmorPlate from the array.
                int randomIndex = Random.Range(0, allArmorPlates.Length);
                ArmorPlate plate = allArmorPlates[randomIndex];

                // Check if the plate is already in the list.
                if (!armorPlates.Contains(plate))
                {
                    armorPlates.Add(plate);
                    plate.gameObject.SetActive(true);
                    addedCount++; // Increment the count of added plates.
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        // Ensure the list is not empty to avoid errors.
        if (armorPlates.Count > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                // Select a random index from the list.
                int randomIndex = Random.Range(0, armorPlates.Count);
                ArmorPlate randomPlate = armorPlates[randomIndex];

                armorPlates.Remove(randomPlate);
                randomPlate.gameObject.SetActive(false);

                health -= amount;
            }
        }
    }
}
