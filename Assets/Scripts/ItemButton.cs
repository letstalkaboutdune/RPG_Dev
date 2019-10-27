// ItemButton
// handles the pull of item details in menu and shop windows

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class ItemButton : MonoBehaviour
{
    // creates various variables to handle item button details
    public Image buttonImage;
    public Text amountText;
    public int buttonValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press() // creates function to pull item details from master array of items when an item is selected in inventory
    {        
        if (GameMenu.instance.theMenu.activeInHierarchy) // checks if normal game menu is open to handle items outside of shop
        {
            if (GameManager.instance.itemsHeld[buttonValue] != "") // checks if itemsHeld array contains an item when an item button is pressed
            {
                GameMenu.instance.SelectItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonValue]), GameManager.instance.numberOfItems[buttonValue]); // calls SelectItem function in Game Manager to pull item details when a particular item button is clicked
            }
            else // executes if selected item is null
            {
                GameMenu.instance.SelectItem(null, 0); // returns null value
            }
        }

        if (Shop.instance.shopMenu.activeInHierarchy) // checks if shop menu is open to handle items inside of shop
        {
            if (Shop.instance.buyMenu.activeInHierarchy) // checks if buy menu is open           
            {
                Shop.instance.SelectBuyItem(GameManager.instance.GetItemDetails(Shop.instance.itemsForSale[buttonValue])); // calls SelectBuyItem to pull item details, based on items for sale array at button value index
            }

            if (Shop.instance.sellMenu.activeInHierarchy) // checks if sell menu is open
            {
                Shop.instance.SelectSellItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonValue]), GameManager.instance.numberOfItems[buttonValue]); // calls SelectBuyItem to pull item details, based on items held array at button value index
            }
        }

        if (BattleManager.instance.itemMenu.activeInHierarchy) // checks if battle item menu is open
        {
            if (GameManager.instance.itemsHeld[buttonValue] != "") // checks if itemsHeld array contains an item when an item button is pressed
            {
                // WIP
                BattleManager.instance.SelectBattleItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonValue])/*, GameManager.instance.numberOfItems[buttonValue]*/); // calls SelectBattleItem function in BattleManager to pull item details when a particular item button is clicked
            }
            else  // executes if selected item is null
            {
                BattleManager.instance.SelectBattleItem(null/*, 0*/); // returns null value
                // END WIP
            }
        }
    }
}