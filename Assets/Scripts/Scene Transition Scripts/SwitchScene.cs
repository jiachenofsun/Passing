using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** A script that defines a zone where the current character can transition to another scene. */
public class SwitchScene : MonoBehaviour
{
    /** Whether there is a character in range of the zone this script is attached to. */
    private bool canSwitch = false;
    /** The ID of the targetLevel. Can be found in Unity's build settings. */
    [SerializeField]
    private int targetLevel;
    /** The X and Y coordinates of where to spawn the character in the target level. */
    [SerializeField]
    private Vector2 targetPos;
    /** If true, scene will switch when someone enters the zone. Else, needs E to be pressed. */
    [SerializeField]
    private bool automatic;

    /** The character that is being transported. */
    private GameObject toTransport;

    /** A reference to the UI_Help. */
    private UI_Help uihelp;

    private void Awake()
    {
        uihelp = FindObjectOfType<UI_Help>();
    }

    private void Update()
    {
        if (!automatic && canSwitch && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Transporting " + toTransport);
            FindObjectOfType<LevelChanger>().FadeToLevel(targetLevel, toTransport, targetPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (automatic)
        {
            if (collision.CompareTag("Playable_Char") || collision.CompareTag("Player"))
            {
                toTransport = collision.gameObject;
                if (collision.CompareTag("Playable_Char") && !toTransport.GetComponent<Playable_Char>().isPossessed())
                {
                    return;
                }
                Debug.Log("Transporting " + toTransport);
                FindObjectOfType<LevelChanger>().FadeToLevel(targetLevel, toTransport, targetPos);
                
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (automatic)
            return;
        if (collision.CompareTag("Playable_Char") || collision.CompareTag("Player"))
        {
            toTransport = collision.gameObject;
            if (collision.CompareTag("Playable_Char") && !toTransport.GetComponent<Playable_Char>().isPossessed())
            {
                return;
            }
            canSwitch = true;
            uihelp.setUIHelp("E", "Press E to enter");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canSwitch = false;
        toTransport = null;
        uihelp.disableUIHelp();
    }

    
}
