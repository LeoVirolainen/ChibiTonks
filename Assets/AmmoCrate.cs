using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrate : MonoBehaviour
{
    float initialY;
    int ammoAmount = 20;
    // Start is called before the first frame update
    void Start()
    {
        initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 50f * Time.deltaTime, 0f);

        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time * 3f) * .3f + initialY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponentInChildren<TankShoot>().ammo += ammoAmount;
            ammoAmount = 0;
        }
        else if (other.CompareTag("Target"))
        {
            if (other.GetComponent<ArmorDestroyer>() != null)
            {
                other.GetComponent<ArmorDestroyer>().ammo += 3;
                ammoAmount -= 3;
            }
        }
        if (ammoAmount <= 0)
            Destroy(gameObject);
    }
}
