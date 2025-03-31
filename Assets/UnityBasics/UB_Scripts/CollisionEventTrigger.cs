using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEventTrigger : MonoBehaviour {
    public UnityEvent myEvent;
    public bool playerOnly;
    private void OnTriggerEnter(Collider other) {
        if (playerOnly == true) {
            if (other.gameObject.GetComponentInChildren<Camera>() != null) {
                myEvent.Invoke();
                return;
            }
        }
        else {
            myEvent.Invoke();
        }
    }
}
