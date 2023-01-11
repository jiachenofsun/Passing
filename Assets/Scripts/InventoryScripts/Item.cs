using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Represents an item in a Playable_Char's inventory. */
[Serializable]
public class Item
{

    public Item(ItemType type, int amount)
    {
        _amount = amount;
        _itemType = type;
    }

    /** Enum of all possible itemTypes */
    public enum ItemType
    {
        Ring,
        Key,
        Cookie,
        Turtle,
    }

    /** My item type. */
    public ItemType _itemType;

    /** The amount of this type of item I have. */
    private int _amount;

    /** Gets the sprite of the item. */
    public Sprite GetSprite()
    {
        switch (_itemType)
        {
            default:
            case ItemType.Ring: return ItemAssets.Instance.ringSprite;
            case ItemType.Key: return ItemAssets.Instance.keySprite;
            case ItemType.Cookie: return ItemAssets.Instance.cookieSprite;
            case ItemType.Turtle: return ItemAssets.Instance.turtleSprite;
        }
    }

    /** Fetches my itemType. */
    public ItemType itemType()
    {
        return _itemType;
    }

    /** Fetches the amount of my itemType. */
    public int amount()
    {
        return _amount;
    }
}
