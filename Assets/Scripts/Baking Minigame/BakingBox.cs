using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingBox : MonoBehaviour
{
    private bool canStart = false;

    [SerializeField]
    private GameObject canvas;
    private GameObject baker;

    private UI_Help uihelp;
    /** A reference to LevelChanger, which stores information about whether the baking is "done". */
    private LevelChanger levelChanger;

    private void Awake()
    {
        uihelp = FindObjectOfType<UI_Help>();
        levelChanger = FindObjectOfType<LevelChanger>();
    }

    private void Update()
    {
        if (canStart && Input.GetKeyDown(KeyCode.E))
        {
            StartMinigame();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Playable_Char") && !levelChanger.bakingCompleted)
        {
            canStart = true;
            baker = collision.gameObject;
            uihelp.setUIHelp("E", "Press E to start baking!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canStart = false;
        baker = null;
        uihelp.disableUIHelp();
    }

    public void StartMinigame()
    {
        canvas.SetActive(true);
        DialogueManager.isActive = true;
        uihelp.disableUIHelp();
    }

    public void EndMinigame()
    {
        canvas.SetActive(false);
        DialogueManager.isActive = false;
        if (!levelChanger.bakingCompleted)
            uihelp.setUIHelp("E", "Press E to start baking!");
    }

    public void GiveCookie()
    {
        //When giving the cookie, also sets bakingCompleted in LevelChanger to true
        levelChanger.bakingCompleted = true;
        Inventory inv = baker.GetComponent<Playable_Char>().getInventory();
        inv.AddItem(new Item(Item.ItemType.Cookie, 1));
        canStart = false;
        uihelp.disableUIHelp();
        baker.GetComponent<DialogueTrigger>().StartOneTimeDialogue(5);
        baker.GetComponent<DialogueTrigger>().setDefaultIndexes(new int[2] { 6, 7 });
        baker = null;
        StartCoroutine(DialogueManager.WaitforDialogue());
    }
}
