using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_RoomTracker : MonoBehaviour
{
    public MapControl m;
    public float raycastDistance = 10f; // How far down the raycast should go
    public LayerMask layerMask; // Optional: Set this in the inspector to filter which objects can be hit

    private void Start()
    {
        if (m == null)
        {
            m = FindObjectOfType<MapControl>().GetComponent<MapControl>();
        }
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        // Perform the raycast
        if (Physics.Raycast(origin, direction, out hit, raycastDistance, layerMask))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<M_RoomIdentifier>() != null)
            {
                var id = hit.collider.gameObject.GetComponent<M_RoomIdentifier>();
                m.activeRoom = id.roomNum;
            }
        }
        else
        {
            Debug.Log("No object hit.");
        }

        // Debugging: Draw the ray in the Scene view
        Debug.DrawRay(origin, direction * raycastDistance, Color.red);
    }
}
