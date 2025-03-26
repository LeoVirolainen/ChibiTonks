using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
    public float jumpForce;
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Rigidbody>() != null) {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
