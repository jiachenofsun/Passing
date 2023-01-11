using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Contains prefabs of all possible Playable_Chars in the game.
 * Keeps track whether DetectionRadiuses, Interactions, Dialogues of all characters are "done" or not.
 * Also keeps track of the Default Dialogues Indexes of all characters.
 * Used with the LevelChanger class to spawn clones of Playable_Chars when changing scenes. */
public class CharManager : MonoBehaviour
{
    /** Contains prefabs of all possible Playable_Chars in the game. */
    public GameObject[] allCharacterGameObjects;


    /** Map of scene IDs to characters currently in. */
    public static Dictionary<int, List<characterState>> inSceneChars = new Dictionary<int, List<characterState>>();

    /** Used to specify which characters belong in which scenes at the beginning of the game.
     * allScenesDefaultCharacters[i] corresponds to Level with ID of i in Unity's Build Settings. */
    [SerializeField]
    private DefaultCharacters[] allScenesDefaultCharacters;

    /** The current scene I am in. */
    private int currScene;

    #region Character memory variables
    /** The "memory" of DetectionRadius, ResistPossession, Dialogues and Interactions for all characters.
     * array[i] corresponds to the entry for The Playable_Character with ID of i. */
    [HideInInspector]
    public bool[] detectionRadiusArray;
    [HideInInspector]
    public bool[] resistPossessionArray;
    public bool[][] dialogueArray;
    public int[][] defaultDialogueArray;
    public bool[][] interactionArray;
    #endregion

    /** Don't destroy on load. */
    public static CharManager instance;

    /** Don't destroy on load. */
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        PopulateWithDefaults();

        detectionRadiusArray = new bool[allCharacterGameObjects.Length];
        resistPossessionArray = new bool[allCharacterGameObjects.Length];
        dialogueArray = new bool[allCharacterGameObjects.Length][];
        defaultDialogueArray = new int[allCharacterGameObjects.Length][];
        interactionArray = new bool[allCharacterGameObjects.Length][];
        ConfigDialogueArray();
        ConfigDefaultDialogueArray();
        ConfigInteractionArray();
    }

    /** Populates the dictionary INSCENECHARS with all the default characters. Called during Awake. */
    private void PopulateWithDefaults()
    {
        for (int i = 0; i < allScenesDefaultCharacters.Length; i++)
        {
            DefaultCharacters scene = allScenesDefaultCharacters[i];
            inSceneChars[i] = new List<characterState>();
            foreach (DefaultCharacter c in scene.defaultCharacters)
            {
                Inventory inv = new Inventory();
                foreach (Item.ItemType x in c.defaultItems)
                    inv.AddItemofType(x);
                //Inventory inv = allCharacterGameObjects[c.charID].GetComponent<Playable_Char>().getInventory();
                addCharToScene(i, c.defaultPosn, c.charID, inv, false);
            }
        }
    }

    /** Configures the Dialogue "memory" array. Called during Awake. */
    private void ConfigDialogueArray()
    {
        for (int i = 0; i < dialogueArray.Length; i++)
        {
            DialogueTrigger curr = allCharacterGameObjects[i].GetComponent<DialogueTrigger>();
            dialogueArray[i] = new bool[curr.numDialogues()];
        }
    }

    /** Configures the Default Dialogue "memory" array. Called during Awake. */
    private void ConfigDefaultDialogueArray()
    {
        for (int i = 0; i < dialogueArray.Length; i++)
        {
            DialogueTrigger curr = allCharacterGameObjects[i].GetComponent<DialogueTrigger>();
            defaultDialogueArray[i] = curr.getOriginalDefaultDialogues();
        }
    }

    /** Configures the Interactions "memory" array. Called during Awake. */
    private void ConfigInteractionArray()
    {
        for (int i = 0; i < dialogueArray.Length; i++)
        {
            CharTalk curr = allCharacterGameObjects[i].GetComponentInChildren<CharTalk>();
            if (curr == null)
            {
                continue;
            }
            interactionArray[i] = new bool[curr.numInteractions()];
        }
    }

    /** Adds a character state to the dictionary INSCENECHARS. */
    public void addCharToScene(int sceneId, Vector2 pos, int id, Inventory inventory, bool play)
    {
        characterState state = new characterState(pos, id, inventory, play);
        inSceneChars[sceneId].Add(state);
    }

    /** Removes a character state from the dictionary INSCENECHARS. */
    public void removeCharFromScene(int sceneId, int charId)
    {
        foreach (characterState cs in inSceneChars[sceneId])
        {
            if (cs.charID == charId)
            {
                inSceneChars[sceneId].Remove(cs);
                return;
            }
        }
    }

    /** Updates a character state to the dictionary INSCENECHARS. */
    public void updateCharPositionInScene(int sceneId, int charId, Vector2 newPos, Inventory newinv, bool player)
    {
        foreach (characterState cs in inSceneChars[sceneId])
        {
            if (cs.charID == charId)
            {
                inSceneChars[sceneId].Remove(cs);
                characterState state = new characterState(newPos, charId, newinv, player);
                inSceneChars[sceneId].Add(state);
                return;
            }
        }
    }

    /** Find all characters in current scene and update the dictionary INSCENECHARS with their current states. */
    public void UpdateAllPositions()
    {
        Playable_Char[] charsInScene = FindObjectsOfType<Playable_Char>();
        foreach (Playable_Char p in charsInScene)
        {
            Vector2 pos = p.gameObject.transform.position;
            updateCharPositionInScene(currScene, p.getID(), pos, p.getInventory(), p.isPossessed());
        }
    }

    /** Spawns all characters in a scene. */
    public void SpawnAll(int sceneId)
    {
        currScene = sceneId;
        Debug.Log("Characters to spawn: " + inSceneChars[sceneId].Count);
        foreach (characterState state in inSceneChars[sceneId])
        {
            Spawn(state.pos, state.charID, state.inventory, state.player);
        }
    }

    /** Spawns a character with ID at POS. */
    public void Spawn(Vector2 pos, int id, Inventory inventory, bool player)
    {
        GameObject character = allCharacterGameObjects[id];
        Object clone = Instantiate(character, pos, character.transform.rotation);
        Playable_Char clonePlayableChar = ((GameObject)clone).GetComponent<Playable_Char>();
        clonePlayableChar.setInventory(inventory);
        if (player)
            clonePlayableChar.PlayableOnNoSound();
    }

    /** Spawns the ghost at (X, Y) */
    public void SpawnGhost(Vector2 pos, GameObject ghost)
    {
        GameObject ghostPlayer = FindObjectOfType<PlayerMovement>().gameObject;
        ghost.transform.position = pos;
    }


    /** A class used to keep track of different characters. Stores information about:
     * their charID, their position, their inventory, and whether they are possessed. */
    public class characterState
    {
        public int charID;
        public Vector2 pos;
        public Inventory inventory;
        public bool player;

        public characterState(Vector2 _pos, int id, Inventory inv, bool play)
        {
            pos = _pos;
            charID = id;
            inventory = inv;
            player = play;
        }
    }

    /** Classes used to specify the default position of characters at the beginning of the game.
     * Eg. the character with ID 0 should be at position (6, -1). */
    [System.Serializable]
    private class DefaultCharacters
    {
        public DefaultCharacter[] defaultCharacters;

    }

    [System.Serializable]
    private class DefaultCharacter
    {
        public int charID;
        public Vector2 defaultPosn;
        public Item.ItemType[] defaultItems;
    }

    public int getCurrScene()
    {
        return currScene;
    }
}
