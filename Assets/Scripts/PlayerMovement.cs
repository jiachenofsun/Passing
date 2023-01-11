using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Movement_variables
    public float movespeed;
    float x_input;
    float y_input;
    #endregion

    #region Physics_components
    Rigidbody2D PlayerRB;
    #endregion

    #region Other_variables
    Vector2 currDirection;
    Collider2D coll;
    private Playable_Char toPossess;
    public static PlayerMovement instance;
    private UI_Help uihelp;
    #endregion

    #region Animation_components
    //Animator anim;
    private SpriteRenderer sr;
    #endregion

    #region Unity_functions
    private void Awake()
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

    private void Start()
    {
        uihelp = FindObjectOfType<UI_Help>();

        PlayerRB = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        //anim = GetComponent<Animator>();

        if (this.gameObject.CompareTag("Playable_Char"))
        {
            this.gameObject.GetComponent<PlayerMovement>().enabled = false;
        }
    }

    private void Update()
    {
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.P))
        {
            Check_Playable();
        }
    }

    private void FixedUpdate()
    {
        if (DialogueManager.isActive || LevelChanger.isLevelChanging || ProgressBar.isActive)
        {
            PlayerRB.velocity = Vector2.zero;
            return;
        }
        Move();
    }
    #endregion

    #region Movement_functions
    private void Move()
    {
        
        //anim.SetBool("Moving", true);
        float correctedspeed = movespeed * Time.fixedDeltaTime * 40f;

        if (x_input > 0)
        {
            PlayerRB.velocity = Vector2.right * correctedspeed;
            currDirection = Vector2.right;
            sr.flipX = false;
        }
        else if (x_input < 0)
        {
            PlayerRB.velocity = Vector2.left * correctedspeed;
            currDirection = Vector2.left;
            sr.flipX = true;

        }
        else if (y_input > 0)
        {
            PlayerRB.velocity = Vector2.up * correctedspeed;
            currDirection = Vector2.up;
        }
        else if (y_input < 0)
        {
            PlayerRB.velocity = Vector2.down * correctedspeed;
            currDirection = Vector2.down;
        }
        else
        {
            PlayerRB.velocity = Vector2.zero;
            //anim.SetBool("Moving", false);
        }
        //anim.SetFloat("DirX", currDirection.x);
        //anim.SetFloat("DirY", currDirection.y);
    }
    #endregion

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Playable_Char"))
        {
            toPossess = collision.gameObject.GetComponent<Playable_Char>();
            uihelp.setUIHelp("P", "Press P to possess " + toPossess.getName());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Playable_Char"))
        {
            toPossess = null;
            uihelp.disableUIHelp();
        }
    }

    private void Check_Playable()
    {
        if (toPossess == null || toPossess.getPossessCounter() >= 0 || DialogueManager.isActive
            || ProgressBar.isActive || LevelChanger.isLevelChanging)
        {
            return;
        }
        uihelp.disableUIHelp();
        toPossess.PlayableOn();
        // disabling this player
        PlayerRB.position = new Vector2(999, 999);
        PlayerRB.velocity = Vector2.zero;
        this.gameObject.GetComponent<Renderer>().enabled = false;
        this.gameObject.GetComponent<PlayerMovement>().enabled = false;

    }

}
