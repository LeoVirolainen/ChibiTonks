using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshOnOff : MonoBehaviour {
    public float interval;
    private MeshRenderer mesh;
    // Start is called before the first frame update
    void Start() {
        mesh = GetComponent<MeshRenderer>();
        StartCoroutine(AlternateOnOff(interval));
    }

    IEnumerator AlternateOnOff(float interval) {
        while (true) {
            yield return new WaitForSeconds(interval);
            if (mesh.enabled) {
                mesh.enabled = false;
            }
            else {
                mesh.enabled = true;
            }
        }
    }
}
