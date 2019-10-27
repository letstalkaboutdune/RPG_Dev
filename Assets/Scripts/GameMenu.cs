// GameMenu
// handles the execution and update of UI and menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class GameMenu : MonoBehaviour
{
    public GameObject theMenu; // creates GameObject for GameMenu

    public GameObject[] windows; // create GameObject array for items, status windows in menu

    private CharStats[] playerStats; // creates CharStats array to handle player stats objects in menu

    public Text[] nameText, hpText, mpText, lvlText, expText; // creates Text array to handle player stats text in main menu
    public Text[] itemNameText, itemHPText, itemMPText; // creates Text array to handle player stats text in item menu

    public Slider[] expSlider; // creates Slider array to handle slider in menu

    public Image[] charImage; // creates Image array to handle image of character in menu

    public GameObject[] charStatHolder; // creates GameObject array to handle state of CharInfo element in menu

    public GameObject[] statusButtons; // creates GameObject array to handle state of status window buttons

    // creates various Text variables to handle values of character stats in stats menu
    public Text statusName, statusHP, statusMP, statusStr, statusDef;
    public Text statusWpnEqpd, statusWpnPwr, statusArmrEqpd, statusArmrPwr, statusLvl, statusExp;

    public Image statusImage; // creates Image variable to handle character image in stats menu

    public ItemButton[] itemButtons; // creates ItemButtons array to manage item button details

    public GameObject itemActionPanel; // creates GameObject to handle display of item action menu

    public GameObject itemNotice; // creates GameObject to handle item notifications
    public Text itemNotificationText; // creates Text to handle item notification text

    public string selectedItem; // creates string to handle name of selected item in inventory

    public Item activeItem, lastItem; // creates Item object to handle active item details
    private int activeItemQuantity; // creates int to handle quantity of selected item in inventory

    public Text itemName, itemDescription, useButtonText; // creates Text variables to handle active item name and description

    // creates GameObject and Text array variables to handle item use menu
    public GameObject itemCharChoiceMenu;
    public Text[] itemCharChoiceNames;

    public static GameMenu instance; // creates reference to this particular instance of this GameMenu class/script

    public Text goldText; // creates Text object to manage current gold

    public string mainMenuName; // creates string object to manage name of main menu scene

    public bool itemNoticeActive = false; // creates bool to handle if item notice is true

    public int soundToPlay; // creates int to handle playback of different sounds

    public GameObject[] expToNextLvl; // creates game object to handle display of EXP sliders

    public GameObject quitConfirmation; // creates game object to handle quit confirmation dialog

    // creates variables to handle game notification dialog box and text
    public GameObject gameNotification;
    public Text notificationText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets GameMenu instance to this instance
                         // "this" keyword refers to current instance of the class
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && !GameManager.instance.battleActive && !GameManager.instance.shopActive && !GameManager.instance.dialogActive && !GameManager.instance.noticeActive) // checks for user to press Fire2 (RMB by default), and battle, shop, dialog, and notice not active
        {
            if (theMenu.activeInHierarchy) // checks if the menu is active
            {
                // theMenu.SetActive(false); // *disabled* hides menu if already active
                // GameManager.instance.gameMenuOpen = false; // *disabled* sets gameMenuOpen tag to false to allow player movement

                CloseMenu(); // calls CloseMenu function to close menu
            }
            else
            {
                theMenu.SetActive(true); // displays menu if not already active

                UpdateMainStats(); // updates stats in menu

                activeItem = null; // sets null active item when menu opens

                GameManager.instance.gameMenuOpen = true; // sets gameMenuOpen tag to true to prevent player movement
            }

            AudioManager.instance.PlaySFX(5); // plays UI beep 2 from audio manager
        }
    }

    public void UpdateMainStats() // creates function to update stats in menu
    {
        playerStats = GameManager.instance.playerStats; // sets playerStats in game menu equal to player stats in game manager

        for (int i = 0; i < playerStats.Length; i++) // creates for loop to iterate through all elements of playerStats array
        {
            if (playerStats[i].gameObject.activeInHierarchy)  // checks if player stats object is active in the scene
            {
                charStatHolder[i].SetActive(true); // activates char info in menu if player is active

                charImage[i].sprite = playerStats[i].charImage; // updates char image in menu data

                nameText[i].text = playerStats[i].charName; // updates char name in menu data
                itemNameText[i].text = playerStats[i].charName; // updates char name in item menu data

                hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP; // updates char HP in menu data
                itemHPText[i].text = playerStats[i].currentHP + "/" + playerStats[i].maxHP; // updates char HP in item menu data

                mpText[i].text = "MP: " + playerStats[i].currentMP + "/" + playerStats[i].maxMP; // updates char MP in menu data
                itemMPText[i].text = playerStats[i].currentMP + "/" + playerStats[i].maxMP; // updates char MP in item menu data

                lvlText[i].text = "Lvl: " + playerStats[i].playerLevel; // updates char LVL in menu data
                expText[i].text = "" + playerStats[i].currentEXP + "/" + playerStats[i].expToNextLevel[playerStats[i].playerLevel]; // updates char LVL in menu data

                if (!playerStats[i].isMaxLevel) // checks if player is not at max level
                {
                    expSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].playerLevel]; // updates char max value of EXP slider in menu data to required EXP to next level
                    expSlider[i].value = playerStats[i].currentEXP; // updates char current value of EXP slider in menu data
                }
                else // executes if player is max level
                {
                    expToNextLvl[i].SetActive(false);
                }

            }
            else
            {
                charStatHolder[i].SetActive(false); // deactivates char info in menu if player is inactive
            }
        }

        goldText.text = GameManager.instance.currentGold.ToString() + "g"; // updates current gold on game menu
    }


    public void ToggleWindow(int windowNumber) // creates function to toggle menu windows on and off
                                               // requires an int reference to windowNumber to run
    {
        UpdateMainStats(); // updates stats in menu
        
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            for (int i = 0; i < windows.Length; i++) // iterates through all elements in the windows array
            {
                if (i == windowNumber) // checks if i is equal to the passed window number
                {
                    windows[i].SetActive(!windows[i].activeInHierarchy); // sets current window element to its opposite state of active/inactive
                }
                else // executes if i is not equal to the passed window number
                {
                    windows[i].SetActive(false); // deactivates current window element
                }

                // WIP
                SelectItem(null, 0); // select null item each time when you toggle the menu window

                itemCharChoiceMenu.SetActive(false); // closes character choice menu when menu is closed
            }
        }
    }

    public void CloseMenu() // creates function to close menu
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            for (int i = 0; i < windows.Length; i++) // loops through all elements in the windows array
            {
                windows[i].SetActive(false); // de-activates all menu windows
            }

            theMenu.SetActive(false); // de-activates menu

            GameManager.instance.gameMenuOpen = false; // sets gameMenuOpen tag to false to allow player movement

            itemCharChoiceMenu.SetActive(false); // closes character choice menu when menu is closed
            activeItem = null; // resets active item to null
        }
    }

    public void OpenStatus() // creates function to open stats window and manage status
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            UpdateMainStats(); // updates stats in menu

            StatusChar(0); // updates the information shown in the stats window

            for (int i = 0; i < statusButtons.Length; i++) // updates the information shown in the stats window
            {
                statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy); // activates button if player object is active in the hierarchy

                statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].charName; // gets Text child component of all buttons in statusButtons array
                                                                                                // updates button text with name of players
            }
        }
    }

    public void StatusChar(int selected) // creates function to update player stats in status window
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            // NEED TO UPDATE WITH NEW STATS
            // WIP
            // updates basic values of selected character in status window
            statusName.text = playerStats[selected].charName;
            statusHP.text = "" + playerStats[selected].currentHP + "/" + playerStats[selected].maxHP;
            statusMP.text = "" + playerStats[selected].currentMP + "/" + playerStats[selected].maxMP;
            statusStr.text = playerStats[selected].strength.ToString();
            //statusDef.text = playerStats[selected].defense.ToString();
            //statusWpnPwr.text = playerStats[selected].wpnPwr.ToString();
            //statusArmrPwr.text = playerStats[selected].armrPwr.ToString();

            if (playerStats[selected].equippedWpn != "") // checks if equipped weapon is not empty
            {
                statusWpnEqpd.text = playerStats[selected].equippedWpn; // updates value of equipped weapon            
            }
            else // executes if weapon value is empty
            {
                statusWpnEqpd.text = "None"; // sets weapon name to None
            }

            if (playerStats[selected].equippedArmr != "") // checks if equipped armor is not empty
            {
                statusArmrEqpd.text = playerStats[selected].equippedArmr; // updates value of equipped armor
            }
            else // executes if armor value is empty
            {
                statusArmrEqpd.text = "None"; // sets armor name to None
            }

            statusExp.text = (playerStats[selected].expToNextLevel[playerStats[selected].playerLevel] - playerStats[selected].currentEXP).ToString(); // updates exp to next level calculating based on current EXP and level
            statusLvl.text = playerStats[selected].playerLevel.ToString(); // sets player level

            statusImage.sprite = playerStats[selected].charImage; // updates character image in status window
        }
    }

    public void ShowItems()     // creates function to assign item button value
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            GameManager.instance.SortItems(); // sorts items before displaying

            for (int i = 0; i < itemButtons.Length; i++) // iterates through all elements in itemButtons
            {
                itemButtons[i].buttonValue = i; // sets item button value to current iteration

                if (GameManager.instance.itemsHeld[i] != "") // checks if element in itemsHeld array is empty
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(true); // sets button image to active

                    itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite; // calls GetItemDetails function in GameManager to determine that item element
                                                                                                                                           // pulls sprite from itemsHeld array in GameManager and assigns to button image
                    itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString(); // pulls number from numberOfItems array in GameManager and assigns to button amount
                }
                else // executes if element in itemsHeld array is blank
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(false); // sets button image to inactive

                    itemButtons[i].amountText.text = ""; // sets amount text to blank
                }
            }
        }
    }

    public void SelectItem(Item newItem, int newItemQuantity) // creates function to update item name, item description, and use button name in inventory when item is selected
                                                              // must pass Item object and int quantity to function
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            if (newItem != null) // checks for null item selection to prevent out-of-range errors
            {
                if (newItem != lastItem) // checks if new selected item is not same as last selected item
                {
                    CloseItemCharChoice(); // closes item character choice panel
                }

                lastItem = newItem; // updates last item check
                activeItem = newItem; // sets passed Item object to activeItem
                activeItemQuantity = newItemQuantity; // sets passed int to activeItemQuantity

                OpenItemActionPanel(); // shows item action panel

                if (activeItem.isItem) // checks if activeItem is of type item
                {
                    useButtonText.text = "Use"; // sets use button text to "Use"
                }

                if (activeItem.isWeapon || activeItem.isArmor) // checks if activeItem is of type weapon or armor
                {
                    useButtonText.text = "Equip"; // sets use button text to "Equip"
                }

                // updates selected item name and description
                itemName.text = activeItem.itemName;
                itemDescription.text = activeItem.description;
            }
            else // executes if selected item is null
            {
                activeItem = null; // resets active item
                CloseItemActionPanel(); // close item action panel

                // resets item text to default
                itemName.text = "Please select an item.";
                itemDescription.text = "";
            }
        }
    }

    public void DiscardItem() // creates function to discard active item in inventory
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            if (activeItem != null) // checks if active item exists
            {
                GameManager.instance.RemoveItem(activeItem.itemName); // calls remove item function in order to discard item
                Debug.Log(activeItem.itemName + " was discarded."); // prints debug text to notify on item discard

                activeItemQuantity--; // decrements active item quantity
                                      // resets item details in menu to default values
                CloseItemCharChoice(); // hides item character choice panel

                if (activeItemQuantity <= 0) // checks if active 
                {
                    activeItem = null; // resets active item to null

                    // resets item details in menu to default values, hides item action panel
                    itemName.text = "Please select an item.";
                    itemDescription.text = "";

                    CloseItemActionPanel(); // hides item action panel
                }
            }
        }
    }

    public void OpenItemCharChoice() // creates function to open item character choice menu and update names
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            if (activeItem != null) // checks for active item to prevent out-of-range errors
            {
                itemCharChoiceMenu.SetActive(true); // activates item character choice menu

                for (int i = 0; i < itemCharChoiceNames.Length; i++) // iterates through all character names
                {
                    itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName; // replaces text of character buttons with player names from player stats objects array

                    itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy); // sets character button active/inactive based on whether character is active in hierarchy
                }
            }
        }
    }


    public void CloseItemCharChoice() // creates function to close item character choice menu
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            itemCharChoiceMenu.SetActive(false); // de-activates item character choice menu
        }
    }

    public void UseItem(int selectChar) // creates function to handle use of item character choice menu buttons
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            itemNoticeActive = false; // resets item notice active to false
            
            //Debug.Log("Game.Use passed item = " + activeItem.name); // prints passed item name to debug log

            activeItem.Use(selectChar); // calls Use function to handle use of item on selected char

            if (itemNoticeActive) // checks if item notice is sent back true from Item.cs script
            {
                StartCoroutine(ShowItemNotice());
                itemNoticeActive = false; // resets itemNoticeActive flag
            }
            else
            {
                Debug.Log(activeItem.itemName + " was used."); // prints debug text to notify on item use

                activeItemQuantity--; // decrements active item quantity
                                      // resets item details in menu to default values
                if (activeItemQuantity <= 0) // checks if active 
                {
                    activeItem = null; // resets active item to null

                    // resets item details in menu to default values
                    itemName.text = "Please select an item.";
                    itemDescription.text = "";

                    CloseItemActionPanel(); // hides item action panel
                }

                CloseItemCharChoice(); // de-activates item character choice menu
                UpdateMainStats(); // updates character stats
            }
        }
    }

    public void SaveGame() // creates function to handle button on menu to save all game data
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            // calls functions to save player data and quest data
            GameManager.instance.SaveData();
            QuestManager.instance.SaveQuestData();

            // sets game notification next and shows panel
            GameMenu.instance.notificationText.text = "Game saved.";
            StartCoroutine(ShowGameNotification());
        }
    }

    public void PlayButtonSound(int soundToPlay) // creates function to play UI beeps, requires int of sound to play
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            AudioManager.instance.PlaySFX(soundToPlay); //; plays UI beep sound from audio manager
        }
    }

    public void QuitGame() // creates function to quit game and load to the main menu
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            SceneManager.LoadScene(mainMenuName); // loads main menu

            // destroys any open objects from currently open scene
            Destroy(GameManager.instance.gameObject);
            Destroy(PlayerController.instance.gameObject);
            Destroy(AudioManager.instance.gameObject);
            Destroy(gameObject);
        }
    }

    public void OpenItemActionPanel() // creates function to handle opening of item menu action panel
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            itemActionPanel.SetActive(true); // shows item action panel
        }
    }

    public void CloseItemActionPanel() // creates function to handle closing of item menu action panel
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            itemActionPanel.SetActive(false); // hides item action panel
        }
    }

    public IEnumerator ShowItemNotice() // creates IEnumerator coroutine to show item notification
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            itemNotice.SetActive(true); // shows item notification panel

            yield return new WaitForSeconds(1f); // forces one second wait for notification to display

            itemNotice.SetActive(false); // shows hides notification panel      
        }
    }

    public void QuitButton() // creates function when pressing quit button
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            quitConfirmation.SetActive(true); // shows quit confirmation dialog
        }
    }

    public void NoButton() // creates function when pressing no button
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            quitConfirmation.SetActive(false); // hides quit confirmation dialog
        }
    }

    public IEnumerator ShowGameNotification() // creates IEnumerator coroutine to show game notification
    {
        GameManager.instance.noticeActive = true; // sets notice active true to stop player action
        GameMenu.instance.gameNotification.SetActive(true); // shows game notification panel

        yield return new WaitForSeconds(1f); // forces one second wait for notification to display

        GameMenu.instance.gameNotification.SetActive(false); // hides game notification panel      
        GameManager.instance.noticeActive = false; // sets notice active false to allow player action
    }
}

