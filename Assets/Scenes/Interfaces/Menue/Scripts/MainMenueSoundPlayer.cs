using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class MainMenueSoundPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public void backButton()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Memes/DiscordLeave");

    }
    public void loadGameButton()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Memes/Bad");

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
