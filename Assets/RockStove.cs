using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockStove : MonoBehaviour
{
    public int amtOfStones;
    public int maxStones = 6;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseUp()
    {
        if (amtOfStones < maxStones)
        {
            amtOfStones++;
        }
    }
    private void OnMouseDown()
    {
        if (amtOfStones > 0)
        {
            amtOfStones--;
        }
    }
}
