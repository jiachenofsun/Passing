using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Script that allows camera to follow the current character.
 * On Start, camera starts following ghost.
 * Position of character relative to the Camera will determine the viewpoint:
 * i.e. if the character is centered, the character will be centered during runtime. */
public class CameraScript : MonoBehaviour
{
    /** Don't destroy on load. */
    public static CameraScript instance;

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

    /** The player that is being followed by the camera. */
    private GameObject player;
    /** The offset between the player's position and the camera's position. */
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }

    /** Makes the camera track NEWPLAYER. */
    public void setPlayer(GameObject newplayer)
    {
        player = newplayer;
    }
}
