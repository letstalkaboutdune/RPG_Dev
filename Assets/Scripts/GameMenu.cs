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

    public Text[] nameText, hpText, spText, lvlText, expText; // creates Text array to handle player stats text in main menu
    public Text[] itemNameText, itemHPText, itemSPText; // creates Text array to handle player stats text in item menu

    // creates slider arrays to handle various menu sliders
    public Slider[] hpSlider, spSlider, expSlider, itemHPSlider, itemSPSlider;

    public Image[] charImage; // creates Image array to handle image of character in menu

    public GameObject[] charStatHolder; // creates GameObject array to handle state of CharInfo element in menu

    public GameObject[] statusButtons; // creates GameObject array to handle state of status window buttons

    // creates various Text variables to handle values of character stats in stats menu
    public Text statusName, statusStatus, statusLV, statusHP, statusSP, statusNextLV, statusAbilityLV, statusNextAbility;
    public Text statusStr, statusTech, statusEnd, statusAgi, statusLuck, statusSpeed;
    public Text statusWeaponDmg, statusHitChance, statusCritChance, statusWeaponDef, statusEvadeChance, statusBlockChance, statusTechDef;
    public Text statusWeaponEquip, statusOffhandEquip, statusArmorEquip, statusAccyEquip;
    public Slider statusHPSlider, statusSPSlider, statusXPSlider, statusAPSlider;
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

    public Text[] abilityList; // creates Text array to handle list of abilities in status menu

    public Toggle[] frontRowToggle; // creates Toggle array to handle player front row toggles

    public Toggle showStatusTooltip; // creates toggle to handle display of status menu tooltips

    // creates variables to manage game timer
    public Text timerText;
    public float secondsCount;
    public int minuteCount, hourCount;

    public GameObject yesNoButtons; // creates game object reference to store yes/no dialog choice buttons
    private bool isRunning = false; // creates private bool to handle if dialog-driven coroutine is running

    // creates variables to manage save window
    public GameObject[] savePanels;
    public Text[] saveLeader, saveLV, saveAreaName, saveTime, saveGold;
    public SavePortraits[] savePortraits;
    public Sprite emptyPortrait, timPortrait, woodyPortrait, sleepyPortrait;

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets GameMenu instance to this instance
                         // "this" keyword refers to current instance of the class
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && !GameManager.instance.battleActive && !GameManager.instance.shopActive && !GameManager.instance.dialogActive && !GameManager.instance.noticeActive && !GameManager.instance.fadingBetweenAreas) // checks for user to press Fire2 (RMB by default), and various game states not active
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

        UpdateTimerUI(); // calls function to update game timer in menu
    }

    public void UpdateMainStats() // creates function to update basic stats in main menu
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

                // updates HP sliders in char info and item window
                hpSlider[i].maxValue = playerStats[i].maxHP;
                hpSlider[i].value = playerStats[i].currentHP;
                itemHPSlider[i].maxValue = playerStats[i].maxHP;
                itemHPSlider[i].value = playerStats[i].currentHP;

                // updates SP sliders in char info and item window
                spSlider[i].maxValue = playerStats[i].maxSP;
                spSlider[i].value = playerStats[i].currentSP;
                itemSPSlider[i].maxValue = playerStats[i].maxSP;
                itemSPSlider[i].value = playerStats[i].currentSP;

                spText[i].text = "SP: " + playerStats[i].currentSP + "/" + playerStats[i].maxSP; // updates char SP in menu data
                itemSPText[i].text = playerStats[i].currentSP + "/" + playerStats[i].maxSP; // updates char SP in item menu data

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

                frontRowToggle[i].isOn = playerStats[i].inFrontRow; // sets front row toggle indicator based on player front row status
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
            // updates status effect of selected char in status window
            if (playerStats[selected].statusEffect != "") // checks if status effect is not empty
            {
                statusStatus.text = playerStats[selected].statusEffect; // updates value of status effect
            }
            else // executes if status effect is empty
            {
                statusStatus.text = ""; // sets status effect to None
            }

            // updates basic stats of selected char in status window
            statusImage.sprite = playerStats[selected].charImage;
            statusName.text = playerStats[selected].charName;
            statusLV.text = playerStats[selected].playerLevel.ToString();
            statusHP.text = "" + playerStats[selected].currentHP + "/" + playerStats[selected].maxHP;
            statusSP.text = "" + playerStats[selected].currentSP + "/" + playerStats[selected].maxSP;
            statusAbilityLV.text = playerStats[selected].playerAPLevel.ToString();
            
            // updates basic stats of selected char in status window
            statusHPSlider.maxValue = playerStats[selected].maxHP;
            statusHPSlider.value = playerStats[selected].currentHP;
            statusSPSlider.maxValue = playerStats[selected].maxSP;
            statusSPSlider.value = playerStats[selected].currentSP;

            if (playerStats[selected].isMaxLevel) // checks if player is max level
            {
                statusNextLV.text = "MAX"; // sets "MAX" text on exp to next level
                statusXPSlider.gameObject.SetActive(false); // hides XP slider
            }
            else // executes if player is not max level
            {
                statusNextLV.text = (playerStats[selected].expToNextLevel[playerStats[selected].playerLevel] - playerStats[selected].currentEXP).ToString(); // sets exp to next level text
                statusXPSlider.maxValue = playerStats[selected].expToNextLevel[playerStats[selected].playerLevel]; // updates char max value of EXP slider in menu data to required EXP to next level
                statusXPSlider.value = playerStats[selected].currentEXP; // updates char current value of EXP slider in menu data
                statusXPSlider.gameObject.SetActive(true); // shows XP slider
            }

            if (playerStats[selected].isMaxAPLevel) // checks if player is max AP level
            {
                statusNextAbility.text = "MAX"; // sets "MAX" test on ap to next level
                statusAPSlider.gameObject.SetActive(false); // hides AP slider
            }
            else // executes if player is not max AP level
            {
                statusNextAbility.text = (playerStats[selected].apToNextLevel[playerStats[selected].playerAPLevel] - playerStats[selected].currentAP).ToString(); // sets AP to next level text
                statusAPSlider.maxValue = playerStats[selected].apToNextLevel[playerStats[selected].playerAPLevel]; // updates char max value of AP slider in menu data to required AP to next level
                statusAPSlider.value = playerStats[selected].currentAP; // updates char current value of AP slider in menu data
                statusAPSlider.gameObject.SetActive(true); // shows AP slider
            }

            // updates main stats of selected character in status window
            statusStr.text = playerStats[selected].strength.ToString();
            statusTech.text = playerStats[selected].tech.ToString();
            statusEnd.text = playerStats[selected].endurance.ToString();
            statusAgi.text = playerStats[selected].agility.ToString();
            statusLuck.text = playerStats[selected].luck.ToString();
            statusSpeed.text = playerStats[selected].speed.ToString();

            // updates derived status of selected char in status window
            statusWeaponDmg.text = playerStats[selected].dmgWeapon.ToString();
            statusHitChance.text = playerStats[selected].hitChance.ToString();
            statusCritChance.text = playerStats[selected].critChance.ToString();
            statusWeaponDef.text = playerStats[selected].defWeapon.ToString();
            statusEvadeChance.text = playerStats[selected].evadeChance.ToString();
            statusBlockChance.text = playerStats[selected].blockChance.ToString();
            statusTechDef.text = playerStats[selected].defTech.ToString();

            // updates equip info of selected char in status window
            if (playerStats[selected].equippedWpn != "") // checks if equipped weapon is not empty
            {
                statusWeaponEquip.text = playerStats[selected].equippedWpn; // updates value of equipped weapon            
            }
            else // executes if weapon value is empty
            {
                statusWeaponEquip.text = "None"; // sets weapon name to None
            }

            if (playerStats[selected].equippedOff != "") // checks if equipped offhand is not empty
            {
                statusOffhandEquip.text = playerStats[selected].equippedOff; // updates value of equipped offhand
            }
            else // executes if offhand value is empty
            {
                statusOffhandEquip.text = "None"; // sets offhand name to None
            }

            if (playerStats[selected].equippedArmr != "") // checks if equipped armor is not empty
            {
                statusArmorEquip.text = playerStats[selected].equippedArmr; // updates value of equipped armor
            }
            else // executes if armor value is empty
            {
                statusArmorEquip.text = "None"; // sets armor name to None
            }

            if (playerStats[selected].equippedAccy != "") // checks if equipped accessory is not empty
            {
                statusAccyEquip.text = playerStats[selected].equippedAccy; // updates value of equipped accessory
            }
            else // executes if accessory value is empty
            {
                statusAccyEquip.text = "None"; // sets accessory name to None
            }

            for(int i = 0; i < playerStats[selected].playerAPLevel; i++) // iterates equal to number of player AP level
            {
                abilityList[i].text = playerStats[selected].abilities[i]; // sets ability list text to currently unlocked abilities
            }

            for(int i = playerStats[selected].playerAPLevel; i < playerStats[selected].maxAPLevel; i++) // iterates through all remaining AP levels
            {
                abilityList[i].text = ""; // sets locked ability text to blank
            }
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

    public void ShowSaveWindow() // creates function to handle showing save menu
    {       
        for(int i = 0; i < 3; i++) // iterates through all 3 possible player prefs slots
        {
            Debug.Log("*** SLOT " + i + " ***"); // prints save slot notice to debug log
            
            List<string> portraitNames = new List<string>(); // creates new list to handle list of active player portraits

            if (PlayerPrefs.HasKey(i + "_Current_Scene")) // checks if save slot exists by checking for current scene key
            {
                savePanels[i].SetActive(true); // shows save panel

                Debug.Log("Current_Scene[" + i + "] = " + PlayerPrefs.GetString(i + "_Current_Scene")); // prints current scene value to debug log

                // grabs save game info from player prefs and updates text
                saveLeader[i].text = PlayerPrefs.GetString(i + "_LeaderName");
                saveLV[i].text = PlayerPrefs.GetString(i + "_LeaderLV");
                saveAreaName[i].text = PlayerPrefs.GetString(i + "_Current_Scene");
                saveGold[i].text = PlayerPrefs.GetInt(i + "_CurrentGold") + "g";
                saveTime[i].text = "" + PlayerPrefs.GetInt(i + "_Hours").ToString("00") + ":" + PlayerPrefs.GetInt(i + "_Minutes").ToString("00") + ":" + PlayerPrefs.GetInt(i + "_Seconds").ToString("00");

                // code below handles setting correct player portraits
                // builds a list of active players
                for (int j = 0; j < playerStats.Length; j++) // iterates through all elements of player stats array
                {
                    if(PlayerPrefs.GetInt(i + "_Player_" + playerStats[j].charName + "_active") != 0) // checks if player active status is not zero
                    {
                        Debug.Log(playerStats[j].charName + " is active in the party."); // prints player active notice                        
                        portraitNames.Add(playerStats[j].charName); // adds active player name to portrait names list
                    }
                    else // executes if player is inactive
                    {
                        Debug.Log(playerStats[j].charName + " is not active in the party."); // prints player not active notice
                    }                    
                }

                // ensures that portrait names list always contains 3 elements
                while(portraitNames.Count < 3) // while loop executes until portrait names list contains 3 elements
                {
                    portraitNames.Add(""); // adds an empty element to list
                }

                // pulls portraits from referenced sprites based on name and assigns to portrait sprite
                for (int j = 0; j < 3; j++) // iterates through 3 times
                {
                    if (portraitNames[j] != "") // checks if list item is not blank
                    {
                        switch (portraitNames[j]) // checks portrait names against known list of players
                        {
                            case "Tim":
                                savePortraits[i].portrait[j].sprite = timPortrait;
                                //Debug.Log("Found Tim.");
                                break;
                            case "Woody":
                                savePortraits[i].portrait[j].sprite = woodyPortrait;
                                //Debug.Log("Found Woody.");
                                break; 
                            case "Sleepy":
                                savePortraits[i].portrait[j].sprite = sleepyPortrait;
                                //Debug.Log("Found Sleepy.");
                                break;
                            default:
                                break;
                        }                        
                    }
                    else // executes if list item is empty
                    {
                        savePortraits[i].portrait[j].sprite = emptyPortrait;
                        //Debug.Log("Empty slot.");
                    }
                }
            }
            else // executes if save slot does not exist
            {
                savePanels[i].SetActive(false); // hides save panel
            }

            portraitNames.Clear(); // clears portrait names list
        }
    }

    public void SaveGame(int saveSlot) // creates function to handle button on menu to save all game data
    {
        if (!GameManager.instance.noticeActive) // checks to see if game notice is active
        {
            // calls functions to save player data and quest data
            GameManager.instance.SaveData(saveSlot);
            QuestManager.instance.SaveQuestData(saveSlot);

            ShowSaveWindow(); // calls show save window function to update stats

            // sets game notification next and shows panel
            notificationText.text = "Game saved.";
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
            Destroy(PlayerController.instance.gameObject);
            Destroy(GameManager.instance.gameObject);
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
    
    public void ToggleFrontRow(int charToToggle) // creates function to handle toggling front row status of character
    {
        //Debug.Log("The value of toggle " + charToToggle + " is " + frontRowToggle[charToToggle].isOn); // prints state of selected toggle
        GameManager.instance.playerStats[charToToggle].inFrontRow = frontRowToggle[charToToggle].isOn; // sets front row state of selected character based on toggle value
    }

    public void UpdateTimerUI() // creates function to track game time and update timer UI
    {
        secondsCount += Time.deltaTime;
        timerText.text = hourCount.ToString("00") + ":" + minuteCount.ToString("00") + ":" + ((int)secondsCount).ToString("00");
        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }
        else if (minuteCount >= 60)
        {
            hourCount++;
            minuteCount = 0;
        }
    }

    public void DialogNoButton() // creates function to handle no button press in dialog menu
    {
        yesNoButtons.SetActive(false); // hides yes/no buttons
        DialogManager.instance.waitForButtons = false; // resets wait for buttons tag to resume dialog
    }

    public void DialogYesButton() // creates function to handle yes button press in dialog menu
    {
        if (isRunning == false) // checks if coroutine is already running
        {
            StartCoroutine(RestAtInn()); // calls rest at inn coroutine
        }
    }

    public IEnumerator RestAtInn() // creates function to rest at inn
    {
        isRunning = true; // sets coroutine true to prevent duplicate calls
        
        // hides yes/no buttons and dialog boxes
        yesNoButtons.SetActive(false);
        DialogManager.instance.dialogBox.SetActive(false);
        DialogManager.instance.nameBox.SetActive(false);
        
        UIFade.instance.FadeToBlack(); // fades screen to black

        GameManager.instance.RestoreHPSP(); // restores party HP/SP
        
        yield return new WaitForSeconds(2f); // waits for 2 seconds
        
        UIFade.instance.FadeFromBlack(); // fades screen from black
        
        // resets dialog-related game tags to resume play
        DialogManager.instance.waitForButtons = false;
        GameManager.instance.dialogActive = false;

        isRunning = false; // sets coroutine false to allow later calls
    }
}

