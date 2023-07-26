using Cinemachine;
using Cinemachine.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VcamHscrim : MonoBehaviour
{
    public SpriteRenderer flipXc;
    CinemachineFramingTransposer trackingX;
    

    void Start()
    {
        var Vcamn = GetComponent<CinemachineVirtualCamera>();
        if ( Vcamn != null )
        {
            trackingX = Vcamn.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
      var fliperX = GetComponent<PlayerMovement>();
      flipXc = fliperX.fliper;
    }


    void Update()
    {
        cameraUptade();
    }

    public void cameraUptade()
    {

        if (flipXc.flipX == false)
        {
            trackingX.m_TrackedObjectOffset.x = Mathf.Lerp(0.2f, 1f, 0.2f);
        }
        else if (flipXc.flipX == true)
        {
            trackingX.m_TrackedObjectOffset.x = Mathf.Lerp(-0.2f, -1f, -0.2f);
        }
    }
}
