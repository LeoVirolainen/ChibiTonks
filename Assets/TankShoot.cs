using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShoot : MonoBehaviour
{
    public GameObject explosion;
    public GameObject muzzleFlash;
    public float velocity;
    public float fireRate;
    float nextFireTime;
    //public Animator gunAnim;
    //public GameObject cnnShotTrail;

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextFireTime)
        {
            //AudioFW.Play("CnnInside");
            StartCoroutine("Shoot");
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator Shoot()
    {
        //ViewChanger.Instance.ShakeCurrentCam(10, 5, 0, 1);
        //gunAnim.SetTrigger("Shoot");
        float waitTime;
        if (muzzleFlash != null)
        {
            GameObject mf = Instantiate(muzzleFlash, transform.position, transform.rotation);
            //Destroy(mf, 2);
        }        
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * 500, Color.white, 1f);
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            waitTime = Vector3.Distance(transform.position, hit.point) / velocity;
            /*CnnShotTrail trail = Instantiate(cnnShotTrail, transform.position, Quaternion.identity).GetComponent<CnnShotTrail>();
            trail.transform.LookAt(hit.point);
            trail.speed = velocity;
            Destroy(trail, 2);*/
            yield return new WaitForSeconds(waitTime);
            GameObject explsn = Instantiate(explosion, hit.point, new Quaternion(hit.transform.rotation.x, transform.rotation.y, hit.transform.rotation.z, hit.transform.rotation.w));
            Destroy(explsn, 0.5f);
        }
    }
}
