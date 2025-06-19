using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_TroopControl : MonoBehaviour
{
    public Animator a;
    public KeyCode doAnimKey;
    public GameObject shootParticle;
    public int animState = 0;
    public Transform barrel;
    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(doAnimKey))
        {
            if (animState == 0)
            {
                a.Play("Troop_Present");
                animState = 1;
            }
            else if (animState == 1)
            {
                a.Play("Troop_FirenLoad");
                Instantiate(shootParticle, barrel.position, barrel.rotation);
                //animState = 0;
            }
        }
    }
}
