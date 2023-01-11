using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/** Controls the UI_Inventory element in the Canvas. */
public class UI_Inventory : MonoBehaviour
{

    #region Variables
    /** The inventory to be displayed. */
    private Inventory inventory;
    /** A UI element which houses all the itemSlotTemplates. */
    private Transform itemSlotContainer;
    /** A UI element representing a template for items on the UI. */
    private Transform itemSlotTemplate;
    /** The player whose inventory is being displayed. */
    private Playable_Char player;
    #endregion

    private void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
    }

    /** Sets player to the specified player, and displays the player's inventory. */
    public void SetPlayer(Playable_Char player)
    {
        this.player = player;
        SetInventory(player.inventory);
    }

    /** Sets inventory to the specified inventory, and displays it on the UI. */
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        // subscribes the inventory's EventHandler to my Inventory_OnItemListChanged method
        inventory.OnItemListChanged += Inventory_OnItemListChanged;

        RefreshInventoryItems();
    }

    /** EventHandler for when inventory is updated. */
    private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    /** Makes the Inventory UI invisible. */
    public void HideInventory()
    {
        transform.localScale = new Vector3(0, 0, 0);
        //gameObject.SetActive(false);
    }

    /** Makes the Inventory UI visible. */
    public void ShowInventory()
    {
        transform.localScale = new Vector3(1, 1, 1);
        //gameObject.SetActive(true);
    }

    /** Displays the contents of inventory on the UI. */
    private void RefreshInventoryItems()
    {
        // deletes all existing itemSlotTemplates
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 92f;
        foreach (Item item in inventory.GetInventory())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, -y * itemSlotCellSize);
            Image image = itemSlotRectTransform.Find("image").GetComponent<Image>();
            image.sprite = item.GetSprite();

            x++;
            if (x >= 5)
            {
                x = 0;
                y++;
            }
        }
    }


}
