using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraManager : MonoBehaviour
{
    private Transform cameraTrafnsform = default;
    private Vector2 originalPosOfCam = default;
    [SerializeField] private float shakeFrequency = default;
    private bool shakeSwitch;

    // Start is called before the first frame update
    void Start()
    {
        shakeSwitch = GetComponentInParent<PlayerGodController>().playerWasHit;
        originalPosOfCam = cameraTrafnsform.position;
    }

    private void Update()
    {
        if (shakeSwitch)
        {
            cameraShakeStart();
        }
        else
        {
            cameraShakeStop();
        }
    }

    public void cameraShakeStart()
    {
        cameraTrafnsform.position = originalPosOfCam + Random.insideUnitCircle * shakeFrequency;
    }

    public void cameraShakeStop()
    {
        cameraTrafnsform.position = originalPosOfCam;
    }
}
