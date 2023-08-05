using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using FMOD.Studio;


public class L0S0TriggerSound : MonoBehaviour
{

    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    private EventInstance CaveBackground;

    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        onTriggerEnter.Invoke();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        onTriggerEnter.Invoke();
    }

    public void SoundPlay()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Backgrounds/L0_S0/CaveBackground");
        
    }

    public void SoundStop()
    {
       
    }
}
