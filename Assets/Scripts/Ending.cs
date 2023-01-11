using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ending : MonoBehaviour
{
    /** The animator that controls the fade in/fade out. */
    [SerializeField]
    private Animator animator;

    /** The messages to be played in the intro scene. */
    [SerializeField]
    private Message[] messages;

    [SerializeField]
    private Actor[] actors;

    private void Awake()
    {
        Destroy(GameObject.FindGameObjectWithTag("MainCamera"));
        Destroy(FindObjectOfType<LevelChanger>().gameObject);
    }
    private void Start()
    {
        
        StartCoroutine(End());
    }

    IEnumerator End()
    {
        yield return new WaitForSeconds(2.5f);
        DialogueManager d = FindObjectOfType<DialogueManager>();
        d.InitDialogue(messages, actors);
        while (DialogueManager.isActive == true)
        {
            yield return null;
        }
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2.5f);
        TextMeshProUGUI endText = GameObject.FindGameObjectWithTag("ThankYouText").GetComponent<TextMeshProUGUI>();
        float duration = 1.5f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, newAlpha);
            yield return null;
        }
    }
}
