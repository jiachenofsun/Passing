using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A Singleton which manages the sprites of all possible item types. */
public class ItemAssets : MonoBehaviour
{
    /** Makes an ItemAssets Singleton. */
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /** Prefab of an item in the overworld, if needed. */
    public Transform pfItemWorld;

    // Drag and drop the sprites of items in the Inspector
    public Sprite keySprite;
    public Sprite ringSprite;
    public Sprite cookieSprite;
    public Sprite turtleSprite;

    // Ingredients for food minigame
    public Sprite ingrASprite;
    public Sprite ingrBSprite;
    public Sprite ingrCSprite;
    public Sprite ingrDSprite;
    public Sprite ingrESprite;
}
