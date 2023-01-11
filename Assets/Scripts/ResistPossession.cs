using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistPossession : MonoBehaviour
{
    private DialogueTrigger trig;
    private Playable_Char character;

    /** The "Destruction Dialogue". Once this index of Dialogue in TRIG is done, 
     * CHARACTER will no longer resist possession. */
    [SerializeField]
    private int destructionDialogue;

    /** The index of Dialogue in TRIG to start if ghost attempts possession. */
    [SerializeField]
    private int dialogueToStart;

    private CharManager charManager;
    private int id;

    private bool routineActive;

    void Start()
    {
        charManager = FindObjectOfType<CharManager>();
        trig = GetComponentInParent<DialogueTrigger>();
        character = GetComponentInParent<Playable_Char>();
        id = character.getID();
    }

    void Update()
    {
        if (charManager.resistPossessionArray[id])
        {
            return;
        }
        if (charManager.dialogueArray[id][destructionDialogue])
        {
            charManager.resistPossessionArray[id] = true;
            return;
        }
        if (character.isPossessed() && !DialogueManager.isActive && !routineActive)
        {
            StartCoroutine(Resist());
        }
    }

    /** Plays the appropriate Dialogue, waits for that Dialogue to finish,
     * then kicks the ghost out! */
    IEnumerator Resist()
    {
        routineActive = true;
        trig.StartDialogue(dialogueToStart);
        while (DialogueManager.isActive)
        {
            yield return null;
        }
        character.PlayableOff();
        routineActive = false;

    }

    public bool isDone()
    {
        return charManager.resistPossessionArray[id];
    }

    public void setisDone(bool x)
    {
        charManager.resistPossessionArray[id] = x;
    }
}
