using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptHolders : MonoBehaviour
{
    /** Don't destroy on load. */
    public static ScriptHolders instance;

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
}
