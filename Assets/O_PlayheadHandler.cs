using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_PlayheadHandler : MonoBehaviour
{
    public O_FactionControl phaseTracker;
    public RectTransform playhead;
    private float phaseLength;
    // Start is called before the first frame update
    void Start()
    {
        //fit available phases into 1000, e.g. 1000 / 6 = 166.6f
        phaseLength = 1000 / phaseTracker.maxPhase;
        print(phaseLength);
    }

    //triggered by FactionControl
    public void MovePlayhead()
    {
        float newPlayheadPos = phaseTracker.currentPhase * phaseLength;
        print(newPlayheadPos);

        Vector2 pos = playhead.anchoredPosition;
        pos.x = newPlayheadPos;
        playhead.anchoredPosition = pos;
    }
}
