using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class MainMenueSoundPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
    }

    public void backButton()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Memes/DiscordLeave");

    }
    public void loadGameButton()
    {

    }
    public void AboutUsButton()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Memes/Idi");
    }
    public void SettingsButton()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Memes/Boom");
    }

}
