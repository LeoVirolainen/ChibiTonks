using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS_Cam;

public class RTSManager : MonoBehaviour
{
    public static bool controllingTank = false;

    public RTS_Camera RTSCam;
    // public RTSCameraController RTSCam;
    // public TankCameraController TankCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (controllingTank)
            {
                controllingTank = false;
                RTSCam.useKeyboardInput = true;
                //RTSCam.enabled = true;
                //TankCam.enabled = false;
            }
            else
            {
                controllingTank = true;
                RTSCam.useKeyboardInput = false;
                //TankCam.enabled = true;
                //RTSCam.enabled = false;
            }
        }
    }
}
