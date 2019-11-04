// ItemButton
// handles the pull of item details in menu and shop windows

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library
using UnityEngine.EventSystems; // includes Unity EventSystems library

public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // creates various variables to handle item button details
    public Image buttonImage;
    public Text amountText;
    public int buttonValue;

    private Tooltip tooltip; // creates reference to tooltip to control tooltip display

    private void Awake()
    {
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>(); // gets reference to tooltip
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
                BattleManager.instance.SelectBattleItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonValue])/*, GameManager.instance.numberOfItems[buttonValue]*/); // calls SelectBattleItem function in BattleManager to pull item details when a particular item button is clicked
            }
            else  // executes if selected item is null
            {
                BattleManager.instance.SelectBattleItem(null/*, 0*/); // returns null value
            }
        }
    }

    // function is called by IPointerEnterHandler whenever a user mouses over an item
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(buttonImage.gameObject.activeInHierarchy) // check if item button image is active in hierarchy, meaning item isn't 
        {
            Item item = null; // creates local item variable to store passed item
            
            if (GameManager.instance.gameMenuOpen || GameManager.instance.battleActive) // checks if game menu or battle menu is open, since both pull from character inventory
            {
                item = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonValue]); // calls GetItemDetails function to pull item from itemsHeld array at button location                
            }
            else if (GameManager.instance.shopActive) // checks if game menu is open
            {
                item = GameManager.instance.GetItemDetails(Shop.instance.itemsForSale[buttonValue]); // calls GetItemDetails function to pull item from itemsForSale array at button location                
            }

            tooltip.GenerateTooltip(item); // calls function to generate and display tooltip based on passed item
        }
    }

    // function is called by IPointerExitHandler whenever a user stops mousing over an item
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false); // hides tooltip
    }
}