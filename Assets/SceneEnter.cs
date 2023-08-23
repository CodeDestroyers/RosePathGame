using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnter : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private int playerExitPoint;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerGodController>())
        {
            PlayerGodController.scenePosition = playerExitPoint;
            SceneSwitchAnimation();
        }
    }

    private void SceneSwitchAnimation()
    {
        animator.Play("SceneSwitchAnim");
    }

    public void SceneSwitcher()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

}
