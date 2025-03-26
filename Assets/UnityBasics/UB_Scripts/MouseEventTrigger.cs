using UnityEngine;
using UnityEngine.Events;

public class MouseEventTrigger : MonoBehaviour
{
    public UnityEvent myEvent;
    void OnMouseDown()
    {
        myEvent.Invoke();
    }
}
