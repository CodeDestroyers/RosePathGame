using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineVirtualCamera[] _VirtualCameras;
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeTreshold = -15f;

    public static CameraManager instance;

    public bool IsLerpingYDamping { get; private set; }

    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _LerpYPanCoroutine;
    private CinemachineFramingTransposer _framingTransporer;
    private CinemachineVirtualCamera _currentCamera;

    private float _normYPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _VirtualCameras.Length; i++)
        {
            if (_VirtualCameras[i].enabled)
            {
                _currentCamera = _VirtualCameras[i];

                _framingTransporer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                
            }
        }

    }

    public void LerpYDamping(bool isPlayerFalling)
    {

    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _framingTransporer.m_YDamping;
        float endDampAmount = 0f;


        if (isPlayerFalling )
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;

        }

        else
        {
            endDampAmount = _normYPanAmount;

        }

        float elaspedTime = 0f;
        while (elaspedTime < _fallYPanTime)
        {
            elaspedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elaspedTime / _fallYPanTime));
            _framingTransporer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }



}
