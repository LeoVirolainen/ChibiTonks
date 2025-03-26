using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Projectile : MonoBehaviour {
    [Tooltip("Should the projectile destroy any object it hits?")]
    public bool killOnHit;
    [Tooltip("Force with which to push the projectile forward when it spawns. 0 = no force.")]
    public float startForce;
    
    private Rigidbody rb;
    private CapsuleCollider col;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }
    private void Start() {
        Destroy(gameObject, 5);
        if (startForce > 0) 
        {
            rb.AddForce(Vector3.forward * startForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (killOnHit)
        {
            var parent = GetHighestParent(collision.gameObject.transform);
            Destroy(parent.gameObject);
        }
        Die();
    }

    void Die() {
        Destroy(gameObject);
    }
    Transform GetHighestParent(Transform obj)
    {
        while (obj.parent != null)
        {
            obj = obj.parent;
        }
        return obj;
    }
}
