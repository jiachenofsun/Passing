using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/** A Singleton which displays messages on the UI. */
public class DialogueManager : MonoBehaviour
{
    #region Serializable_variables
    /** A reference to the Avatar image on the UI. */
    [SerializeField]
    private Image actorImage;

    /** A reference to the NameText gameObject on the UI. */
    [SerializeField]
    private GameObject actorName;

    /** A reference to the MessageText gameObject on the UI. */
    [SerializeField]
    private GameObject messageText;

    /** A reference to the DialogueBox on the UI. */
    [SerializeField]
    private RectTransform backgroundBox;

    /** How many milisecs pass between each letter being typed out. */
    [SerializeField]
    private float milisecsBtwLetters;
    #endregion

    #region Variables
    /** An array of messages to be displayed on the UI. */
    private Message[] currentMessages;

    /** An array of actors to be displayed on the UI. */
    private Actor[] currentActors;

    /** Integer which tracks what the current message is. */
    private int activeMessage = 0;

    /** Boolean which returns whether a Dialogue is taking place. */
    public static bool isActive = false;

    /** Boolean which returns whether you can advance to the next message in a dialogue. */
    private bool canAdvance;

    /** A coroutine representing which sentence is currently being typed onto the UI. */
    private IEnumerator currRoutine;

    /** The AudioManager in the scene. */
    private AudioManager audioManager;

    /** Reference to the UI_Help. */
    private UI_Help uihelp;
    #endregion

    #region Dialogue_functions
    /** Initializes a dialogue using the given arrays of messages and actorIDs. */
    public void InitDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;
        DisplayMessage();
        // Uses the LeanTween package to make the dialogue box transition in
        backgroundBox.LeanScale(Vector3.one, 0.3f).setEaseInOutExpo();
    }

    /** Displays the next message on the UI,
     * and the sprite of the actor saying the message. */
    void DisplayMessage()
    {
        Message m = currentMessages[activeMessage];
        if (activeMessage > 0)
        {
            StopCoroutine(currRoutine);
        }
        currRoutine = TypeMessage(m.message());
        StartCoroutine(currRoutine);

        Actor a = currentActors[m.actorID()];
        actorName.GetComponent<TextMeshProUGUI>().text = a.name();
        actorImage.sprite = a.sprite();
     
    }

    /** Coroutine to type out the current message on the UI letter by letter. */
    IEnumerator TypeMessage(string message)
    {
        uihelp.disableUIHelp();
        canAdvance = false;
        TextMeshProUGUI messageTMP = messageText.GetComponent<TextMeshProUGUI>();
        messageTMP.text = "";
        foreach (char letter in message.ToCharArray())
        {
            audioManager.Play("DialogueBlip");
            messageTMP.text += letter;
            yield return new WaitForSeconds(milisecsBtwLetters / 1000);
        }
        canAdvance = true;
        uihelp.setUIHelp("Space", "Press Space to advance dialogue");
    }

    /** Updates the UI to display the next message in the dialogue.
     * If at end of message, close the dialogue box. */
    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        } else
        {
            // Uses the LeanTween package to make the dialogue box transition out
            StopCoroutine(currRoutine);
            backgroundBox.LeanScale(Vector3.zero, 0.3f).setEaseInOutExpo();
            uihelp.disableUIHelp();
            isActive = false;
        }
    }
    #endregion

    #region Unity_functions
    void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
        audioManager = FindObjectOfType<AudioManager>();
        uihelp = FindObjectOfType<UI_Help>();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isActive && canAdvance)
        {
            NextMessage();
        }

    }
    #endregion

    public static void setIsActive(bool x)
    {
        isActive = x;
    }

    /** A Coroutine that waits for the dialogue to finish, before updating
 * the hearts */
    public static IEnumerator WaitforDialogue()
    {
        while (DialogueManager.isActive == true)
        {
            yield return null;
        }
        FindObjectOfType<ProgressBar>().newTaskCompleted();
    }
}
