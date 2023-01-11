using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A Singleton which manages which Playable_Char's inventory
 * is being displayed by the UI. */
public class CurrentInventory : MonoBehaviour
{
    /** The UI_Inventory in the Canvas with the UI_Inventory component. */
    [SerializeField] private UI_Inventory uiInventory;

    /** The current player. */
    private GameObject currPlayer;

    void Start()
    {
        HideInventory();
    }

    /** Makes the Inventory UI invisible. */
    public void HideInventory()
    {
        uiInventory.HideInventory();
    }

    /** Makes the Inventory UI visible. */
    public void ShowInventory()
    {
        uiInventory.ShowInventory();
    }

    /** Sets currPlayer to the specified player,
     * and updates the UI to reflect their inventory. */
    public void SetPlayer(GameObject player)
    {
        currPlayer = player;
        uiInventory.SetPlayer(currPlayer.transform.GetComponent<Playable_Char>());
    }
}
