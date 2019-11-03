// Shop
// handles shop UI management and item buy/sell execution

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class Shop : MonoBehaviour
{
    public static Shop instance; // creates reference to this particular instance of this Shop class/script
                                 // "static" property only allows one version of this instance for every script in the project
    public GameObject shopMenu, buyMenu, sellMenu, buyActionPanel, sellActionPanel; // creates GameObjects to handle opening/closing menus in shop

    public Text goldText; // creates Text object to handle gold in shop menu

    public string[] itemsForSale; // creates string array to handle items for sale in shop menu

    public ItemButton[] buyItemButtons, sellItemButtons; // creates ItemButton array to handle items for sale in buy and sell menus

    // creates Item and Text objects to manage buy and sell item details in menu
    public Item selectedItem;
    private int selectedItemQuantity;
    public Text buyItemName, buyItemDescription, buyItemValue;
    public Text sellItemName, sellItemDescription, sellItemValue;

    public GameObject goldNotice; // creates GameObject to handle display of not enough gold notice

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets Shop instance to this instance
                         // "this" keyword refers to current instance of the class
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && !GameManager.instance.gameMenuOpen && !GameManager.instance.battleActive && !GameManager.instance.dialogActive && !GameManager.instance.noticeActive) // checks for user to press Fire2 (RMB by default), and battle, shop, dialog, and notice not active
        {
            if (shopMenu.activeInHierarchy) // checks if the shop menu is active
            {
                CloseShop(); // calls function to close the shop
            }
        }
    }

    public void OpenShop() // creates function to handle opening shop
    {
        // activates shop to buy menu and sets shop active bool to prevent player movement
        shopMenu.SetActive(true);
        OpenBuyMenu();
        GameManager.instance.shopActive = true;

        goldText.text = GameManager.instance.currentGold.ToString() + "g"; // updates gold text in shop menu from game manager current gold

        GameMenu.instance.PlayButtonSound(5); // plays sound to open shop
    }

    public void CloseShop() // creates function to handle closing shop
    {
        // de-activates shop menu and resets shop active bool to prevent player movement
        shopMenu.SetActive(false);
        GameManager.instance.shopActive = false;

        GameMenu.instance.PlayButtonSound(5); // plays sound to close shop
    }

    public void OpenBuyMenu() // creates function to handle opening buy menu
    {
        // WIP
        SelectBuyItem(null); // select null item each time when you first open the buy menu

        // sets bools appropriately to open buy menu and close sell menu
        buyMenu.SetActive(true);
        sellMenu.SetActive(false);

        // for loop below based on ShowItems function in GameMenu
        // iterates through all elements in itemButtons
        for (int i = 0; i < buyItemButtons.Length; i++)
        {
            buyItemButtons[i].buttonValue = i; // sets item button value to current iteration

            if (itemsForSale[i] != "") // checks if element in itemsForSale array is empty
            {
                buyItemButtons[i].buttonImage.gameObject.SetActive(true); // sets button image to active

                buyItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(itemsForSale[i]).itemSprite; // calls GetItemDetails function in GameManager to determine that item element
                                                                                                                        // pulls sprite from itemsForSale array in GameManager and assigns to button image
                buyItemButtons[i].amountText.text = ""; // sets number of items for purchase to blank, to allow unlimited item quantities for sale
            }
            else // executes if element in itemsHeld array is blank
            {
                buyItemButtons[i].buttonImage.gameObject.SetActive(false); // sets button image to inactive

                buyItemButtons[i].amountText.text = ""; // sets amount text to blank
            }
        }
    }

    public void OpenSellMenu() // creates function to handle opening sell menu
    {
        // WIP
        SelectSellItem(null, 0); // select null item each time when you first open the sell menu

        // sets bools appropriately to close buy menu and open sell menu
        buyMenu.SetActive(false);
        sellMenu.SetActive(true);

        GameManager.instance.SortItems(); // sorts items in inventory just in case

        // for loop below based on ShowItems function in GameMenu
        // iterates through all elements in itemButtons
        for (int i = 0; i < sellItemButtons.Length; i++)
        {
            sellItemButtons[i].buttonValue = i; // sets item button value to current iteration

            if (GameManager.instance.itemsHeld[i] != "") // checks if element in itemsHeld array is empty
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(true); // sets button image to active

                sellItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite; // calls GetItemDetails function in GameManager to determine that item element
                                                                                                                                           // pulls sprite from itemsHeld array in GameManager and assigns to button image
                sellItemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString(); // pulls number from numberOfItems array in GameManager and assigns to button amount
            }
            else // executes if element in itemsHeld array is blank
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(false); // sets button image to inactive

                sellItemButtons[i].amountText.text = ""; // sets amount text to blank
            }
        }
    }

    private void ShowSellItems() // creates function to update displayed items in sell menu
    {
        GameManager.instance.SortItems(); // sorts items in inventory just in case

        // for loop below based on ShowItems function in GameMenu
        // iterates through all elements in itemButtons
        for (int i = 0; i < sellItemButtons.Length; i++)
        {
            sellItemButtons[i].buttonValue = i; // sets item button value to current iteration

            if (GameManager.instance.itemsHeld[i] != "") // checks if element in itemsHeld array is empty
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(true); // sets button image to active

                sellItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite; // calls GetItemDetails function in GameManager to determine that item element
                                                                                                                                           // pulls sprite from itemsHeld array in GameManager and assigns to button image
                sellItemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString(); // pulls number from numberOfItems array in GameManager and assigns to button amount
            }
            else // executes if element in itemsHeld array is blank
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(false); // sets button image to inactive

                sellItemButtons[i].amountText.text = ""; // sets amount text to blank
            }
        }
    }

    public void SelectBuyItem(Item buyItem) // creates function to handle item selection in buy menu
                                            // requires an Item object passed to execute
    {
        if (buyItem != null) // checks for null item selection to prevent out-of-range errors
        {
            selectedItem = buyItem; // assigns selected item as passed item from buy menu

            OpenBuyActionPanel(); // open buy item action panel

            // updates buy item details in menu based on passed item details
            buyItemName.text = selectedItem.itemName;
            buyItemDescription.text = selectedItem.description;
            buyItemValue.text = "Value: " + selectedItem.value + "g";
        }
        else // executes if selected buy item is null
        {
            CloseBuyActionPanel(); // close buy item action panel

            // resets item text to default
            buyItemName.text = "Welcome to the shop!";
            buyItemDescription.text = "Please select an item.";
            buyItemValue.text = "Value: ";
        }

    }
    
    public void SelectSellItem(Item sellItem, int sellItemQuantity) // creates function to handle item selection in sell menu
                                                                    // requires an Item object and int passed to execute
    {
        if (sellItem != null) // checks for null item selection to prevent out-of-range errors
        {
            // assigns selected item as passed item from sell menu
            selectedItem = sellItem;
            selectedItemQuantity = sellItemQuantity;

            OpenSellActionPanel(); // open sell item action panel

            // updates sell item details in menu based on passed item details
            sellItemName.text = selectedItem.itemName;
            sellItemDescription.text = selectedItem.description;
            sellItemValue.text = "Value: " + Mathf.FloorToInt(selectedItem.value * 0.5f).ToString() + "g"; // scales sell value to half of buy value, truncates to whole number
        }

        else // executes if selected sell item is null
        {
            CloseSellActionPanel(); // close buy item action panel
            
            // resets item text to default
            sellItemName.text = "Welcome to the shop!";
            sellItemDescription.text = "Please select an item.";
            sellItemValue.text = "Value: ";
        }

    }

    public void BuyItem() // creates function to handle item buy in buy menu
    {
        if(selectedItem != null) // checks that selected item slot is not empty
        {
            if (GameManager.instance.currentGold >= selectedItem.value) // checks if player has enough gold to buy item based on current gold and selected item value
            {
                GameManager.instance.currentGold -= selectedItem.value; // subtracts value of item from player's current gold

                GameManager.instance.AddItem(selectedItem.itemName); // adds item into our game manager inventory
            }
            else // executes if player doesn't have enough gold in their inventory
            {
                StartCoroutine(ShowGoldNotice()); // displays not enough gold notification
            }
        }
        
        goldText.text = GameManager.instance.currentGold.ToString() + "g"; // updates gold display after item has been bought
    }

    public void SellItem() // creates function to handle item sell in sell menu
    {
        if (selectedItem != null) // checks that selected item slot is not empty
        {
            GameManager.instance.currentGold += Mathf.FloorToInt(selectedItem.value * 0.5f); // adds sale price of item to current gold in game manager

            GameManager.instance.RemoveItem(selectedItem.itemName); // removes selected item from inventory

            selectedItemQuantity--; // decrements quantity of item sold

            if(selectedItemQuantity <= 0) // checks if item sold quantity is <= 0
            {
                selectedItem = null; // nulls out selected item to prevent duplicate sales
                CloseSellActionPanel(); // close sell item action panel

                // resets sell item details in menu to default values
                sellItemName.text = "Welcome to the shop!";
                sellItemDescription.text = "Please select an item.";
                sellItemValue.text = "";
            }
        }

        goldText.text = GameManager.instance.currentGold.ToString() + "g"; // updates gold display after item has been sold

        ShowSellItems(); // calls show sell items function to update sell item list in menu
    }

    public void OpenBuyActionPanel() // creates function to handle opening of shop buy menu action panel
    {
        buyActionPanel.SetActive(true); // shows item action panel
    }

    public void CloseBuyActionPanel() // creates function to handle closing of shop buy menu action panel
    {
        buyActionPanel.SetActive(false); // hides item action panel
    }

    public void OpenSellActionPanel() // creates function to handle opening of shop buy menu action panel
    {
        sellActionPanel.SetActive(true); // shows item action panel
    }

    public void CloseSellActionPanel() // creates function to handle closing of shop buy menu action panel
    {
        sellActionPanel.SetActive(false); // hides item action panel
    }

    public IEnumerator ShowGoldNotice() // creates IEnumerator coroutine to show not enough gold notice
    {
        goldNotice.SetActive(true); // shows item notification panel

        yield return new WaitForSeconds(1f); // forces one second wait for notification to display

        goldNotice.SetActive(false); // shows item notification panel      
    }
}