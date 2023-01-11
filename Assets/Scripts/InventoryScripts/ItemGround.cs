using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A class representing items that are physically present in the overworld */
public class ItemGround : MonoBehaviour
{
    /** Spawns an item in the specified position */
    public static ItemGround SpawnItem(Vector3 position, Item item)
    {
        Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);

        ItemGround itemGround = transform.GetComponent<ItemGround>();
        itemGround.SetItem(item);

        return itemGround;
    }

    /** The item associated with the gameObject in the overworld */
    private Item _item;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /** Sets the _item variable and the sprite of the gameObject */
    public void SetItem(Item item)
    {
        this._item = item;
        spriteRenderer.sprite = item.GetSprite();
    }

    /** Gets the item associated with the gameObject in the overworld */
    public Item item()
    {
        return _item;
    }

    /** Destroys this gameObject */
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
