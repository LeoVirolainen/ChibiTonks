using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatStone : MonoBehaviour
{
    public float temp;
    public float heatUpSpeed = 1;
    bool isInFire;
    bool isBeingDragged;
    bool isOverTub;
    public Vector3 startPos;

    void Start()
    {
        temp = Mathf.Clamp(temp, 0, 50);
        startPos = transform.position;
    }
    void Update()
    {
        if (isInFire)
        {
            temp += Time.deltaTime * heatUpSpeed;
            temp = Mathf.Clamp(temp, 0, 50);
        }
        else
        {
            temp -= Time.deltaTime * heatUpSpeed;
            temp = Mathf.Clamp(temp, 0, 50);
        }
        if (isBeingDragged)
        {
            // Get mouse position in screen space
            Vector3 mouseScreenPosition = Input.mousePosition;

            // Convert screen position to world position (assuming Z = 0 for a 2D plane)
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Camera.main.nearClipPlane // Adjust if needed; for orthographic, nearClipPlane is fine
            ));

            // Set the object's position (preserve Z position)
            transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
        }
        if (temp != 0 || temp != 50)
        {
            var mat = GetComponent<MeshRenderer>().material;
            mat.color = new Color(temp / 50f, 0f, 0f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Stove")
        {
            isInFire = true;
        }
        /*if (other.gameObject.GetComponent<BathTub>() != null)
        {
            isOverTub = true;
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Stove")
        {
            isInFire = false;
        }
        /*if (other.gameObject.GetComponent<BathTub>() != null)
        {
            isOverTub = false;
        }*/
    }
    private void OnMouseDown()
    {
        isBeingDragged = true;
    }
    private void OnMouseUp()
    {
        isBeingDragged = false;
        if (isOverTub)
        {
            //other.gameObject.GetComponent<BathTub>().temperature++;
            Destroy(gameObject);
        }
        if (!isInFire)
        {
            transform.position = startPos;
        }        
    }
}
