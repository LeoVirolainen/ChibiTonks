using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_TestDotP : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get forward vector in world space
        Vector3 viewDir = transform.TransformDirection(Vector3.forward);

        //get vector pointing to target
        Vector3 toOther = Vector3.Normalize(target.position - transform.position);

        Debug.DrawRay(transform.position, viewDir);
        Debug.DrawRay(transform.position, toOther);

        var dot = Vector3.Dot(viewDir, toOther);

        print(dot);

        if (dot > 0.75f)
        {
            print("I CAN SEE "+ target.name);
        }
    }
}
