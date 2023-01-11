using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A script which controls the locked door in the game. */
public class KeyTransition : MonoBehaviour
{
    /** Booleans that control whether the door functions as a "sign" or a scene transition zone. */
    private bool canSwitch = false;
    private bool canRead = false;

    /** The ID of the targetLevel. Can be found in Unity's build settings. */
    [SerializeField]
    private int targetLevel;
    /** The X and Y coordinates of where to spawn the character in the target level. */
    [SerializeField]
    private Vector2 targetPos;

    /** The character that is being transported. */
    private GameObject toTransport;

    /** If a human walks up to the door, this will store their inventory to check. */
    private Inventory invToCheck;

    /** A reference to the UI_Help. */
    private UI_Help uihelp;

    /** A reference to LevelChanger, which stores information about whether the door is "unlocked". */
    private LevelChanger levelChanger;

    /** A reference to DialogueManager. */
    private DialogueManager dialogueManager;

    /** Input the messages to be displayed on the UI here. */
    [SerializeField]
    private Message[] readMessages;

    [SerializeField]
    private Message[] unlockMessages;

    [SerializeField]
    private Actor[] actors;

    /** True if a message is being displayed on the UI. */
    private bool msgActive;

    private void Awake()
    {
        uihelp = FindObjectOfType<UI_Help>();
        levelChanger = FindObjectOfType<LevelChanger>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        if (DialogueManager.isActive)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canSwitch)
            {
                Debug.Log("Transporting " + toTransport);
                levelChanger.FadeToLevel(targetLevel, toTransport, targetPos);
            }
            else if (canRead && invToCheck != null)
            {
                if (invToCheck.CheckAndRemove(Item.ItemType.Key) == true)
                {
                    dialogueManager.InitDialogue(unlockMessages, actors);
                    levelChanger.doorUnlocked = true;
                }
                else
                {
                    dialogueManager.InitDialogue(readMessages, actors);
                }
            }
            else if (canRead)
            {
                dialogueManager.InitDialogue(readMessages, actors);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Playable_Char") || collision.CompareTag("Player"))
        {
            if (levelChanger.doorUnlocked)
            {
                canSwitch = true;
                toTransport = collision.gameObject;
                uihelp.setUIHelp("E", "Press E to enter");
            } else
            {
                if (collision.CompareTag("Playable_Char")
                && !collision.gameObject.GetComponent<Playable_Char>().isPossessed())
                {
                    return;
                }
                canRead = true;
                uihelp.setUIHelp("E", "Press E to interact");
                if (collision.CompareTag("Playable_Char"))
                {
                    invToCheck = collision.gameObject.GetComponent<Playable_Char>().getInventory();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canSwitch = false;
        canRead = false;
        invToCheck = null;
        toTransport = null;
        uihelp.disableUIHelp();
    }

}
