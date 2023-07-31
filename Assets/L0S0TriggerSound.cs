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
        PLAYBACK_STATE playbackState;
        CaveBackground.getPlaybackState(out playbackState);

        CaveBackground.start();
    }

    public void SoundStop()
    {
        CaveBackground.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
