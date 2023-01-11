using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Script that changes scenes and triggers the fade in/out animations.
 * Always attached to the LevelChanger prefab.
 * Also stores informations about whether events in levels are completed. */
public class LevelChanger : MonoBehaviour
{
    /** The audioManager. */
    private AudioManager audioManager;
    /** The animator that controls the fade in/fade out. */
    public Animator animator;
    /** The level that we're changing to. */
    private int targetLevel;

    /** The first level that is ever loaded at the start of the game. */
    public static int firstLevel = 1;

    /** The level I am currently in. */
    private int currLevel;

    /** The x and y coordinates of where the character will spawn in the target level. */
    private Vector2 targetPos;

    /** Represents whether the ghost is transitioning between scenes or not. */
    private bool transportingPlayer = false;

    /** If a Playable_Char is being transported, stores information about them for use in other functions in the script. */
    private int id;
    private GameObject playerToTransport;
    private Inventory inventory;

    private CharManager charManager;
    private GameObject ghost;

    /** Cooldown for changing scenes. I currently hard-coded it to be 2 seconds. */
    private float cooldownTimer;
    private float cooldown = 2f;
    public static bool isLevelChanging;

    /** Don't destroy on load. */
    public static LevelChanger instance;

    #region Levelevents memory variables
    /** Tracks whether the cemetery complete zone is "done". */
    [HideInInspector]
    public bool CemeteryCompleted;
    [HideInInspector]
    public bool bakingCompleted;
    [HideInInspector]
    public bool doorUnlocked;
    #endregion

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
    }

    /** On start, loads the first scene and initalizes some variables. */
    private void Start()
    {
        charManager = FindObjectOfType<CharManager>();
        currLevel = firstLevel;
        cooldownTimer = cooldown;
        ghost = FindObjectOfType<PlayerMovement>().gameObject;
        audioManager = FindObjectOfType<AudioManager>();
        LoadFirstScene();
    }

    private void Update()
    {
        //Increments cooldownTimer
        if (cooldownTimer < cooldown)
            cooldownTimer += Time.deltaTime;
    }

    /** Fades current scene out, then transports TOTRANSPORT to position (X, Y) at scene LEVELINDEX. */
    public void FadeToLevel(int levelIndex, GameObject toTransport, Vector2 pos)
    {
        if (cooldownTimer < cooldown)
        {
            Debug.Log("Changing scenes is on cooldown!");
            return;
        }
        isLevelChanging = true;
        cooldownTimer = 0;

        targetLevel = levelIndex;
        targetPos = pos;
        playerToTransport = toTransport;

        //UPDATE ALL POSITIONS
        FindObjectOfType<CharManager>().UpdateAllPositions();

        animator.SetTrigger("FadeOut");
        if (toTransport.CompareTag("Playable_Char"))
        {
            transportingPlayer = true;
            id = toTransport.GetComponent<Playable_Char>().getID();
            inventory = toTransport.GetComponent<Playable_Char>().getInventory();
        } else
        {
            transportingPlayer = false;
        }
    }

    /** Runs after current scene fades out. Function is called from inside the animator of LevelChanger. */
    public void onFadeComplete()
    {
        //This avoids a bug whereby because the ghost is DoNotDestroy, it would trigger a DetectionRadius when moved to the new scene
        ghost.transform.position = new Vector2(999f, 999f);
        StartCoroutine(ChangeScene());
    }

    /** Changes current scene to TARGETLEVEL, sets the position of the character to (TARGETX, TARGETY). */
    public IEnumerator ChangeScene()
    {
        if (transportingPlayer) {
            charManager.addCharToScene(targetLevel, targetPos, id, inventory, true);
            charManager.removeCharFromScene(currLevel, id);
        }
        yield return new WaitForSeconds(0.25f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetLevel);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        switch (targetLevel)
        {
            case 1: audioManager.PlaySoundtrack("Cemetery"); break;
            case 2: audioManager.PlaySoundtrack("Town"); break;
            case 3: audioManager.PlaySoundtrack("Interior"); break;
            case 4: audioManager.PlaySoundtrack("Interior"); break;
            case 5: audioManager.PlaySoundtrack("Cemetery"); break;
            default: break;
        }
        
        currLevel = targetLevel;
        charManager.SpawnAll(targetLevel);
        animator.SetTrigger("FadeIn");
        if (!transportingPlayer)
            charManager.SpawnGhost(targetPos, playerToTransport);
        isLevelChanging = false;
    }

    /** Loads the FIRSTLEVEL, by telling the CHARMANAGER to spawn all characters inside FIRSTLEVEL. */
    public void LoadFirstScene()
    {
        charManager.SpawnAll(firstLevel);
        audioManager.PlaySoundtrack("Cemetery");
    }
}
