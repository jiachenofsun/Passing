using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A script which controls inter-character dialogue,
 * and special interactions which occur between specific characters. */
public class CharTalk : MonoBehaviour
{
    /** An array of special interactions, and the corresponding dialogue indexes to trigger. */
    [SerializeField]
    private Interaction[] interactions;

    /** References to the gameObjects of myself, 
     * and the Candidate (aka the person who is approaching me. */
    private GameObject candidate;
    private bool candidatePos;
    private GameObject self;

    private CharManager charManager;
    private int id;
    private string myName;

    /** A reference to the UI_Help. */
    private UI_Help uihelp;

    private void Awake()
    {
        uihelp = FindObjectOfType<UI_Help>();
    }

    private void Start()
    {
        self = this.transform.parent.gameObject;
        charManager = FindObjectOfType<CharManager>();
        id = GetComponentInParent<Playable_Char>().getID();
        myName = GetComponentInParent<Playable_Char>().getName();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Playable_Char"))
        {
            candidate = collision.gameObject;
            Playable_Char candidatePC = candidate.GetComponent<Playable_Char>();
            string candidateName = candidatePC.getName();
            candidatePos = candidatePC.isPossessed();
            if (candidatePos)
                uihelp.setUIHelp("E", "Press E to talk to " + myName);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        candidate = null;
        candidatePos = false;
        uihelp.disableUIHelp();
    }

    /** If there's someone to talk to, press E to start a dialogue! */
    private void Update()
    {
        if (candidate != null && candidatePos && Input.GetKeyDown(KeyCode.E) && !DialogueManager.isActive
            && !LevelChanger.isLevelChanging && !ProgressBar.isActive)
        {
            int candidateID = candidate.GetComponent<Playable_Char>().getID();

            int i = 0;
            /** Attempts to look for a special interaction. */
            foreach (Interaction interact in interactions)
            {
                if (interact.otherID() == candidateID || interact.otherID() == -1)
                {
                    /** If this interaction is done, continue */
                    if (charManager.interactionArray[id][i])
                    {
                        Debug.Log("This interaction is already completed.");
                        continue;
                    }

                    /** Checks if all required items are present */
                    if (!checkItems(interact.myRequiredItems(), interact.theirRequiredItems())) {
                        continue;
                    }

                    //if (!checkScene(interact.sceneID()))
                    //{
                    //    Debug.Log("This interaction can't take place in this scene");
                    //    continue;
                    //}
                    
                    DialogueTrigger trig = GetComponentInParent<DialogueTrigger>();
                    Debug.Log(trig);
                    trig.StartOneTimeDialogue(interact.indexToTrigger());
                    /** Sets my default indexes */
                    if (interact.newDefaults().Length != 0)
                    {
                        Debug.Log("Setting new defaults...");
                        trig.setDefaultIndexes(interact.newDefaults());
                    }
                    self.GetComponentInChildren<DetectionRadius>().setDone();

                    /** Gives all specified items */
                    StartCoroutine(GiveItems(interact.myGivenItems(), interact.theirGivenItems()));

                    if (interact.fade())
                    {
                        StartCoroutine(Fade(interact.heartEvent()));
                    }
                    else if (interact.heartEvent())
                    {
                        StartCoroutine(DialogueManager.WaitforDialogue());
                    }
                    /** Sets this interaction to "done" */
                    charManager.interactionArray[id][i] = true;
                    return;
                }
                i++;
            }

            /** If no special interaction, defaults to default dialogue. */
            GetComponentInParent<DialogueTrigger>().StartDefaultDialogue();
        }
    }

    /** Gives items to the appropriate characters. */
    public IEnumerator GiveItems(Item.ItemType[] myItems, Item.ItemType[] theirItems)
    {
        while (DialogueManager.isActive)
        {
            yield return null;
        }
        giveItems(myItems, self);
        giveItems(theirItems, candidate);
    }

    /** Returns true if required items are present from both SELF and CANDIDATE. */
    private bool checkItems(Item.ItemType[] needFromMe, Item.ItemType[] needFromThem)
    {
        Inventory theirInventory = candidate.GetComponent<Playable_Char>().getInventory();
        Inventory myInventory = self.GetComponent<Playable_Char>().getInventory();
        if (!checkItemsOwner(needFromMe, myInventory) || !checkItemsOwner(needFromThem, theirInventory))
        {
            return false;
        }
        removeItems(needFromMe, myInventory);
        removeItems(needFromThem, theirInventory);
        return true;
    }

    /** Returns true if REQUIREDITEMS are present in this particular INVENTORY. */
    private bool checkItemsOwner(Item.ItemType[] requiredItems, Inventory inventory)
    {
        if (requiredItems == null)
        {
            return true;
        }
        List<Item.ItemType> invList = inventory.GetItemTypeList();
        foreach (Item.ItemType item in requiredItems)
        {
            if (!invList.Contains(item)) {
                return false;
            }
        }
            return true;
    }

    /** Removes ITEMS from INVENTORY. Meant to be called only if requireditem checks for both me and them pass. */
    private void removeItems(Item.ItemType[] items, Inventory inventory)
    {
        foreach (Item.ItemType item in items)
        {
            inventory.RemoveItemofType(item);
        }
    }

    /** Gives the items in ITEMS to CHARACTER. */
    private void giveItems(Item.ItemType[] items, GameObject character)
    {
        Inventory inv = character.GetComponent<Playable_Char>().getInventory();
        foreach (Item.ItemType itemtype in items)
        {
            inv.AddItemofType(itemtype);
        }
    }

    /** Makes the screen fade out and back in. */
    private IEnumerator Fade(bool heartEvent)
    {
        Animator animator = FindObjectOfType<LevelChanger>().animator;
        while (DialogueManager.isActive)
        {
            yield return null;
        }
        Debug.Log("Starting fade");
        if (heartEvent)
        {
            ProgressBar.isActive = true;
        }
        animator.SetTrigger("FadeOutNoSwitch");
        yield return new WaitForSeconds(1);
        animator.SetTrigger("FadeIn");
        if (heartEvent)
        {
            FindObjectOfType<ProgressBar>().newTaskCompleted();
        }
        /** FOR THE TURTLE */
        if (id == 5)
        {
            charManager.removeCharFromScene(charManager.getCurrScene(), id);
            Destroy(self);
            Debug.Log("Destroyed the turtle");
        }
    }

    /** The number of special interactions I have. */
    public int numInteractions()
    {
        return interactions.Length;

    }

    /** Checks the current scene I am in. */
    public bool checkScene(int sid)
    {
        return sid == FindObjectOfType<CharManager>().getCurrScene();
    }



    /** Represents a special interaction. Contains three parameters:
     * the ID of the other character,
     * the index of this character's dialogue to trigger,
     * and (optionally) a set of new Default Dialogue indexes. */
    [System.Serializable]
    public class Interaction
    {
        /** the ID of the other character that triggers this interaction. */
        [SerializeField]
        private int _otherID;
        /** the index of this character's Dialogue[] that this interaction will trigger. */
        [SerializeField]
        private int _indexToTrigger;
        /** Optional: An array specifying the indexes of the new default dialogues after this interaction triggers. */
        [SerializeField]
        private int[] _newDefaults;
        /** if true, screen will fade out and in upon completing the interaction. */
        [SerializeField]
        private bool _fade;
        /** Optional: items required from ME for this interaction to occur. */
        [SerializeField]
        private Item.ItemType[] _myRequiredItems;
        /** Optional: items required from THEM for this interaction to occur. */
        [SerializeField]
        private Item.ItemType[] _theirRequiredItems;
        /** Optional: items given to ME after this interaction occurs. */
        [SerializeField]
        private Item.ItemType[] _myGivenItems;
        /** Optional: items given to THEM after this interaction occurs. */
        [SerializeField]
        private Item.ItemType[] _theirGivenItems;
        /** Optional: the sceneID that we must be in for this interaction to occur. */
        [SerializeField]
        private int _sceneID;
        /** If true, the player will gain a heart upon completion of the interaction! */
        [SerializeField]
        private bool _heartEvent;

        public int otherID()
        {
            return _otherID;
        }

        public int indexToTrigger()
        {
            return _indexToTrigger;
        }

        public int[] newDefaults()
        {
            return _newDefaults;
        }

        public bool fade()
        {
            return _fade;
        }
        public Item.ItemType[] myRequiredItems()
        {
            return _myRequiredItems;
        }

        public Item.ItemType[] theirRequiredItems()
        {
            return _theirRequiredItems;
        }

        public Item.ItemType[] myGivenItems()
        {
            return _myGivenItems;
        }
        public Item.ItemType[] theirGivenItems()
        {
            return _theirGivenItems;
        }
        public int sceneID()
        {
            return _sceneID;
        }
        public bool heartEvent()
        {
            return _heartEvent;
        }
    }
}
