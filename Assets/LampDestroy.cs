using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampDestroy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("PlayerWeapon"))
        {
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
