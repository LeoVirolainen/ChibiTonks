using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnable;
    public float interval;
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
            Instantiate(spawnable, transform.position, transform.rotation);
        }
    }
}
