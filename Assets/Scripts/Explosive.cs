using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public float fuseTime;
    float timeToBlow;
    public string targetTag = "Player";
    public GameObject explosionFx;
    // Start is called before the first frame update
    void Start()
    {
        timeToBlow = Time.time + fuseTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeToBlow)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other) //note: in grenades, trigger is in the 'Detonator' child
    {
        if (other.CompareTag(targetTag)) {
            Explode();
            print("OUCH!");
        }
    }

    void Explode()
    {
        var effect = Instantiate(explosionFx, transform.position, Quaternion.identity);
        Destroy(effect, 1);
        Destroy(gameObject);
    }
}
