using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Represents the inventory of a Playable_Char. */
public class Inventory
{
    public Inventory()
    {
        _inventory = new List<Item>();

        //// TODO: Remove once done testing
        //Item a = new Item(Item.ItemType.Key, 1);
        //AddItem(a);
    }

    #region Variables
    /** EventHandler to update UI when an item is picked up/used. */
    public event EventHandler OnItemListChanged;

    /** List used to store all my items. */
    private List<Item> _inventory;
    #endregion

    #region Inventory_methods
    /** Adds an item to inventory. */
    public void AddItem(Item item)
    {
        _inventory.Add(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddItemofType(Item.ItemType itemtype)
    {
        Item item = new Item(itemtype, 1);
        AddItem(item);
    }

    /** Removes an item from inventory. */
    public void RemoveItem(Item item)
    {
        _inventory.Remove(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    /** Removes an item of type ITEMTYPE from inventory. */
    public void RemoveItemofType(Item.ItemType itemtype)
    {
        foreach (Item inventoryItem in _inventory)
        {
            if (inventoryItem.itemType() == itemtype)
            {
                _inventory.Remove(inventoryItem);
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    /** Checks if an item of type ITEMTYPE is present. if it is, remove it! */
    public bool CheckAndRemove(Item.ItemType itemtype)
    {
        foreach (Item inventoryItem in _inventory)
        {
            if (inventoryItem.itemType() == itemtype)
            {
                _inventory.Remove(inventoryItem);
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }
        return false;
    }

    /** Gets my inventory. */
    public List<Item> GetInventory()
    {
        return _inventory;
    }

    /** Gets a list of items in my inventory in ItemType form. */
    public List<Item.ItemType> GetItemTypeList()
    {
        List<Item> invList = GetInventory();
        List<Item.ItemType> itemTypeList = new List<Item.ItemType>();
        foreach (Item item in invList)
        {
            itemTypeList.Add(item.itemType());
        }
        return itemTypeList;
    }
    #endregion
}
