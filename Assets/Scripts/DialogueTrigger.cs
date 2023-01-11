using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A class managing the contents of a piece of dialogue, as well as who
 * is speaking each line. */
public class DialogueTrigger : MonoBehaviour
{
    /** An array of all possible dialogues. */
    [SerializeField]
    private Dialogue[] dialogues;
    /** An array of actors in this dialogue. */
    [SerializeField]
    private Actor[] actors;

    /** An array of indexes in DIALOGUES that will be looped through for default dialogue.
     * eg. For looping through indexes 0, 1, and 2 of DIALOGUES,
     *     DEFAULTINDEXES will be the array {0, 1, 2}. */
    [SerializeField]
    private int[] defaultIndexes;

    /** Tracks the index in DEFAULTINDEXES. Ranges from 0 to (defaultIndexes.Length - 1) */
    private int defaultPointer = 0;

    private CharManager charManager;
    private int id;

    private DialogueManager dialogueManager;

    private void Start()
    {
        charManager = FindObjectOfType<CharManager>();
        id = GetComponentInParent<Playable_Char>().getID();
    }

    /** Advances DEFAULTPOINTER. */
    private void incrDefaultPointer()
    {
        defaultPointer = (defaultPointer + 1) % charManager.defaultDialogueArray[id].Length;
    }

    /** Sets DEFAULT INDEXES to NEWINDEXES. */
    public void setDefaultIndexes(int[] newIndexes)
    {
        charManager.defaultDialogueArray[id] = newIndexes;
        defaultPointer = 0;
    }

    /** Starts the ith conservation in DIALOGUES. If done, defaults to default dialogue. Displays the dialogue on the UI. */
    public void StartDialogue(int i)
    {

        if (charManager.dialogueArray[id][i])
        {
            StartDefaultDialogue();
        } else
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
            dialogueManager.InitDialogue(getDialogue(i), actors);
        }
    }

    /** Starts the ith conservation in DIALOGUES. Then, sets that conversation to DONE, so that it can never be triggered again. */
    public void StartOneTimeDialogue(int i)
    {
        StartDialogue(i);
        charManager.dialogueArray[id][i] = true;
    }

    /** Starts the next designated default dialogue in DIALOGUES. Displays the dialogue on the UI. */
    public void StartDefaultDialogue()
    {
        int i = charManager.defaultDialogueArray[id][defaultPointer];
        
        dialogueManager = FindObjectOfType<DialogueManager>();
        dialogueManager.InitDialogue(getDialogue(i), actors);
        incrDefaultPointer();
    }

    /** Gets the ith indexed dialogue in DIALOGUES. */
    private Message[] getDialogue(int i)
    {
        return dialogues[i].messages();
    }

    /** Gets the ith indexed dialogue in DIALOGUES. */
    public Dialogue getWholeDialogue(int i)
    {
        return dialogues[i];
    }

    public int numDialogues()
    {
        return dialogues.Length;
    }

    public int[] getOriginalDefaultDialogues()
    {
        return defaultIndexes;
    }
}

/** Represents a message, with actorID specifying who is saying the message. */
[System.Serializable]
public class Message
{
    [SerializeField]
    private int _actorID;
    [SerializeField]
    private string _message;

    /** Gets my actorID. */
    public int actorID()
    {
        return _actorID;
    }

    /** Gets my message. */
    public string message()
    {
        return _message;
    }
}

/** Represents a series of Messages (aka a dialogue). */
[System.Serializable]
public class Dialogue
{
    [SerializeField]
    private Message[] _messages;

    /** Gets the messages in this conversation. */
    public Message[] messages()
    {
        return _messages;
    }
}

/** Represents a character that is able to speak dialogue. */
[System.Serializable]
public class Actor
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private Sprite _sprite;

    /** Gets the actor's name. */
    public string name()
    {
        return _name;
    }

    /** Gets the actor's sprite. */
    public Sprite sprite()
    {
        return _sprite;
    }
}