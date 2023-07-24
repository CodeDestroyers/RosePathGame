using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
 public static AudioManager instance
    {
        get; private set;
    }
    private void Awake()
    {
        if (instance != null) 
        {
            Debug.LogError("Найдено больше одного аудио-менеджера в сцене");
        }
        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}
