using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class HittedObject : MonoBehaviour
{
    public Sprite[] objectState;
    private SpriteRenderer thisSprite;

    private void Awake()
    {
        thisSprite = GetComponent<SpriteRenderer>();
        gameObject.SetActive(true);
        thisSprite.sprite = objectState[0];
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("PlayerWeapon"))
        {
            thisSprite.sprite = objectState[1];
            AudioManager.instance.PlayOneShot(FMODEvents.instance.objectDestroy1, this.transform.position);
            StartCoroutine(ObjectBreack());
        }
    }

    private IEnumerator ObjectBreack()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.05f);
        gameObject.SetActive(false);
    }
}
