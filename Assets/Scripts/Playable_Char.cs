using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable_Char : MonoBehaviour
{
    /** The Playable_Char's ID. Unique to this Playable_Char. */
    [SerializeField]
    private int id;
    [SerializeField]
    private string characterName;

    #region Movement_variables
    public float movespeed;
    private Vector2 currDirection;
    float x_input;
    float y_input;
    #endregion

    #region Random_movement
    /** Determines whether this Playable_Char can move randomly when not possessed. */
    public bool canMove;
    public int randomSeed;
    public float waitTime;
    public float moveTime;
    private float waitCounter;
    public float eachMoveTime;
    private float moveCounter;
    private int MoveDirection;
    #endregion

    #region Other
    private GameObject ghostPlayer;
    /** Determines whether this Playable_Char is controllable currently. */
    private bool player = false;
    private AudioManager audioManager;
    #endregion

    #region Possession Counters
    private float possessDelay = 1;
    private float possessCounter;
    #endregion

    #region Inventory_variables
    public Inventory inventory = new Inventory();
    private CurrentInventory currentInventory;
    #endregion

    #region Physics_components
    Rigidbody2D CharRB;
    #endregion

    #region Animation_variables
    public Animator animator;
    private SpriteRenderer sr;
    public GameObject possessionGlow;
    private GameObject activeGlow;
    #endregion

    private void Awake()
    {
        ghostPlayer = FindObjectOfType<PlayerMovement>().gameObject;
        audioManager = FindObjectOfType<AudioManager>();
        CharRB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
        moveCounter = eachMoveTime;
        waitCounter = waitTime + moveTime;
        possessCounter = possessDelay;
        Random.InitState(randomSeed);

        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", false);

        currentInventory = FindObjectOfType<CurrentInventory>();
    }

    private void Move()
    {
        animator.SetBool("Moving", true);
        float correctedspeed;
        if (!player && canMove)
        {
            correctedspeed = 0.3f * movespeed * Time.fixedDeltaTime * 40f;
        } else
        {
            correctedspeed = movespeed * Time.fixedDeltaTime * 40f;
        }
        if (x_input > 0)
        {
            CharRB.velocity = Vector2.right * correctedspeed;
            currDirection = Vector2.right;
            animator.SetFloat("dirX", 1);
            animator.SetFloat("dirY", 0);
            sr.flipX = false;
        }
        else if (x_input < 0)
        {
            CharRB.velocity = Vector2.left * correctedspeed;
            currDirection = Vector2.left;
            animator.SetFloat("dirX", -1);
            animator.SetFloat("dirY", 0);
            sr.flipX = true;
        }
        else if (y_input > 0)
        {
            CharRB.velocity = Vector2.up * correctedspeed;
            currDirection = Vector2.up;
            animator.SetFloat("dirX", 0);
            animator.SetFloat("dirY", 1);
        }
        else if (y_input < 0)
        {
            CharRB.velocity = Vector2.down * correctedspeed;
            currDirection = Vector2.down;
            animator.SetFloat("dirX", 0);
            animator.SetFloat("dirY", -1);
        }
        else
        {
            CharRB.velocity = Vector2.zero;
            animator.SetBool("Moving", false);
        }

        
    }

    private void RandomizeMove()
    {
        moveCounter -= Time.deltaTime;
        waitCounter -= Time.deltaTime;
        if (moveCounter >= 0)
        {
            if (MoveDirection == 0)
            {
                x_input = 1;
                y_input = 0;

            }
            if (MoveDirection == 1)
            {
                x_input = -1;
                y_input = 0;

            }
            if (MoveDirection == 2)
            {
                x_input = 0;
                y_input = 1;

            }

            if (MoveDirection == 3)
            {
                x_input = 0;
                y_input = -1;

            }
            if (MoveDirection == 4)
            {
                x_input = 0;
                y_input = 0;

            }
        } else
        {
            MoveDirection = Random.Range(0, 4);
            moveCounter = eachMoveTime;
        }

        if (!canMove || waitCounter <= waitTime && waitCounter > 0)
        {
            MoveDirection = 4;
        }
        if (waitCounter < 0)
        {
            waitCounter = waitTime + moveTime;

        }
        
    }

    public void PlayableOff()
    {
        currentInventory.HideInventory();
        this.player = false;

        Destroy(activeGlow);

        CharRB.mass = CharRB.mass * 1000000;
        x_input = y_input = 0;
        Vector2 currPos = this.gameObject.transform.position;
        ghostPlayer.transform.position = currPos;
        ghostPlayer.GetComponent<Renderer>().enabled = true;
        ghostPlayer.GetComponent<PlayerMovement>().enabled = true;
        
        possessCounter = possessDelay;
        audioManager.Play("Possess");
        FindObjectOfType<CameraScript>().setPlayer(ghostPlayer);
        GetComponentInChildren<CharTalk>().enabled = true;

    }

    public void PlayableOn()
    {
        PlayableOnNoSound();
        audioManager.Play("Possess");
    }

    public void PlayableOnNoSound()
    {
        currentInventory = FindObjectOfType<CurrentInventory>();
        currentInventory.SetPlayer(this.gameObject);
        currentInventory.ShowInventory();
        player = true;
        
        possessCounter = possessDelay;
        FindObjectOfType<CameraScript>().setPlayer(this.gameObject);
        GetComponentInChildren<CharTalk>().enabled = false;

        activeGlow = Instantiate(possessionGlow, this.gameObject.transform.position, Quaternion.identity) as GameObject;
        activeGlow.transform.parent = this.gameObject.transform;

        CharRB.mass = CharRB.mass / 1000000;
        //This "twitches" the character when possessed. Used to counter an annoying bug involving SwitchScene. */
        CharRB.velocity = new Vector2(0.0001f, 0);
            
    }


    // Update is called once per frame
    private void Update()
    {
        if (possessCounter >= 0)
        {
            possessCounter -= Time.deltaTime;
        }
        if (player)
        {
            x_input = Input.GetAxisRaw("Horizontal");
            y_input = Input.GetAxisRaw("Vertical");
        }
        else
        {
            RandomizeMove();
        }

        //Programming horror, but I'm doing this because Unity bugs out inconsistently for some reason :((
        if (player && Input.GetKeyDown(KeyCode.P) && possessCounter < 0 && !DialogueManager.isActive
            && !ProgressBar.isActive && !LevelChanger.isLevelChanging)
        {
            PlayableOff();
        }
    }

    private void FixedUpdate()
    {
        if (DialogueManager.isActive || LevelChanger.isLevelChanging || ProgressBar.isActive)
        {
            CharRB.velocity = Vector2.zero;
            animator.SetBool("Moving", false);
            return;
        }
        Move();
    }

    public float getPossessCounter()
    {
        return possessCounter;
    }

    public int getID()
    {
        return id;
    }

    public string getName()
    {
        return characterName;
    }

    public Inventory getInventory()
    {
        return inventory;
    }

    public void setInventory(Inventory newInventory)
    {
        inventory = newInventory;
    }

    public bool isPossessed()
    {
        return player;
    }
}







