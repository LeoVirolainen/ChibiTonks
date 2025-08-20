using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class O_PlayheadHandler : MonoBehaviour
{
    public O_FactionControl phaseTracker;
    public RectTransform playhead;
    private float phaseLength;
    private Vector2 velocity = Vector2.zero;

    private Vector2 newPos;
    // Start is called before the first frame update
    void Start()
    {
        //fit available phases into 1000, e.g. 1000 / 6 = 166.6f
        phaseLength = 1000 / phaseTracker.maxPhase;
        print(phaseLength);

        newPos = playhead.anchoredPosition;
    }

    private void Update()
    {
        if (newPos != null)
        {
            playhead.anchoredPosition = Vector2.SmoothDamp(playhead.anchoredPosition, newPos, ref velocity, 0.3F);
        }
    }

    //triggered by FactionControl
    public void MovePlayhead()
    {
        float newPlayheadX = phaseTracker.currentPhase * phaseLength;
        print(newPlayheadX);

        newPos = playhead.anchoredPosition;    //newPos = current playhead position (vector2)
        newPos.x = newPlayheadX;               //manipulate newPos.x                
    }    
}
