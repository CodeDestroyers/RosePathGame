using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

        var fliperX = GetComponent<PlayerGodController>();

        flipXc = fliperX.playerSprite;

    }

    void Update()
    {
        cameraUptade();
    }

    public void cameraUptade()
    {

        if (flipXc.transform.localScale.x != -1)
        {
            trackingX.m_TrackedObjectOffset.x = Mathf.Lerp(0.2f, 2f, 0.4f);
        }
        else if (flipXc.transform.localScale.x == -1)
        {
            trackingX.m_TrackedObjectOffset.x = Mathf.Lerp(-0.2f, -2f, 0.4f);
        }
    }
}
