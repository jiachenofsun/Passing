using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConstantsCanvas : MonoBehaviour
{
    /** Don't destroy on load. */
    public static UIConstantsCanvas instance;

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
