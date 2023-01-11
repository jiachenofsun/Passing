using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The script controlling the zone that Henry has to enter to be "helped out of the cemetery. */
public class CemeteryCompleteZone : MonoBehaviour
{
    /** A reference to LevelChanger, which stores information about whether the cemetery complete zone is "done". */
    private LevelChanger levelChanger;

    private void Start()
    {
        levelChanger = FindObjectOfType<LevelChanger>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Playable_Char") && !done())
        {
            if (collision.gameObject.GetComponent<Playable_Char>().getID() == 0)
            {
                setDone();
                collision.gameObject.GetComponent<DialogueTrigger>().StartDialogue(1);
                StartCoroutine(DialogueManager.WaitforDialogue());
            }
        }
    }

    /** True if the cemetery complete zone is "done" */
    private bool done()
    {
        return levelChanger.CemeteryCompleted;
    }

    /** Sets the cemetery complete zone to "done" */
    private void setDone()
    {
        levelChanger.CemeteryCompleted = true;
    }
}
