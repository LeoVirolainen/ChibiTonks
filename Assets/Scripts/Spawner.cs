using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnable;
    public float interval;
    public float spawnRadius; // The radius within which the spawnable will be spawned
    float nextSpawnTime;
    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            nextSpawnTime = Time.time + interval;

            // Calculate a random position within the radius
            Vector3 randomPosition = transform.position + (Random.insideUnitSphere * spawnRadius);
            randomPosition.y = transform.position.y; // Ensure the Y position remains the same

            // Spawn the object at the random position
            Instantiate(spawnable, randomPosition, transform.rotation);
        }
    }
}
