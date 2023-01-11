using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A script controlling a Sign:
 * An object that you can approach and click E to display a series of messages on the UI. */
public class Sign : MonoBehaviour
{
    /** Whether there is a character in range of the Sign. */
    private bool canStart = false;

    /** Whether a Sign is currently displaying messages. Used to "pause" the game. */
    public static bool isActive;

    /** The messages to be displayed. */
    [SerializeField]
    private Message[] messages;

    [SerializeField]
    private Actor[] actors;

    /** References to UI_Help and DialogueManager. */
    private UI_Help uihelp;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        uihelp = FindObjectOfType<UI_Help>();
    }

    private void Update()
    {
        if (DialogueManager.isActive)
            return;

        if (canStart && Input.GetKeyDown(KeyCode.E))
        {
            canStart = false;
            dialogueManager = FindObjectOfType<DialogueManager>();
            dialogueManager.InitDialogue(messages, actors);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Playable_Char") || collision.CompareTag("Player"))
        {
            if (collision.CompareTag("Playable_Char")
                && !collision.gameObject.GetComponent<Playable_Char>().isPossessed())
            {
                return;
            }
            canStart = true;
            uihelp.setUIHelp("E", "Press E to read");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canStart = false;
        uihelp.disableUIHelp();
    }
}
