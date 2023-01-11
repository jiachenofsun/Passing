using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuIntro : MonoBehaviour
{
    /** The animator that controls the fade in/fade out. */
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject MainMenuPanel;

    [SerializeField]
    private GameObject IntroPanel;

    /** The messages to be played in the intro scene. */
    [SerializeField]
    private Message[] messages;

    [SerializeField]
    private Actor[] actors;

    /** The AudioManager in the scene. */
    private AudioManager audioManager;

    public void StartGame()
    {
        StartCoroutine(MainMenuToIntro());
    }

    IEnumerator MainMenuToIntro()
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2.5f);
        MainMenuPanel.SetActive(false);
        IntroPanel.SetActive(true);
        animator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2.5f);
        DialogueManager d = FindObjectOfType<DialogueManager>();
        d.InitDialogue(messages, actors);
        while (DialogueManager.isActive == true)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.75f);
        Animator introAnimator = IntroPanel.GetComponent<Animator>();
        introAnimator.SetTrigger("SpawnTombstone");
        yield return new WaitForSeconds(2.5f);
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2.5f);
        if (audioManager != null)
            audioManager.Play("SpawnIn");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(LevelChanger.firstLevel);
    }

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
}
