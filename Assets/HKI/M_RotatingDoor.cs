using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_RotatingDoor : MonoBehaviour
{
    public MapControl m;
    public int myRoom;
    // Start is called before the first frame update
    void Start()
    {
        if (m == null)
        {
            m = FindObjectOfType<MapControl>().GetComponent<MapControl>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m.mapActive && m.activeRoom == myRoom)
        {
            transform.Rotate(0, .2f, 0);
        }
    }
}
