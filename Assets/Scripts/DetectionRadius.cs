using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRadius : MonoBehaviour
{
    /** A boolean which enforces that the event only happens once. */
    [SerializeField]
    private int dialogueToStart = 0;
    private CharManager charManager;
    private int id;

    private void Start()
    {
        charManager = FindObjectOfType<CharManager>();
        id = GetComponentInParent<Playable_Char>().getID();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If collisions is a character, AND this detectionradius is not "done"
        if ((collision.transform.CompareTag("Player") || collision.transform.CompareTag("Playable_Char"))
            && !charManager.detectionRadiusArray[id])
        {
            charManager.detectionRadiusArray[id] = true;
            GetComponentInParent<DialogueTrigger>().StartDialogue(dialogueToStart);
        }
    }

    public void setDone()
    {
        charManager.detectionRadiusArray[id] = true;
    }
}
