﻿// GameManager
// handles manipulation of player stats, items, and gold, ability for player to move, and data save/load

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // creates reference to this particular instance of this GameManager class/script
                                        // "static" property only allows one version of this instance for every script in the project
    public CharStats[] playerStats; // creates CharStats array to handle all our player stats

    public bool gameMenuOpen, dialogActive, fadingBetweenAreas, shopActive, battleActive, noticeActive; // creates bools to manage UI and player movement under certain conditions

    public string[] itemsHeld; // creates string array to handle item names in inventory

    public int[] numberOfItems; // creates int array to handle item quantities in inventory

    public Item[] referenceItems; // creates Item array to handle detailed information for each item

    public int currentGold; // creates int variable to handle current gold

    public int saveSlot; // creates int to manage save slot

    // Start is called before the first frame update
    void Start()
    {
        instance = this;  // sets GameManager instance to this instance
                          // "this" keyword refers to current instance of the class
        DontDestroyOnLoad(gameObject); // calls DontDestroyOnLoad for GameManager to preserve it across all scenes

        SortItems(); // sort items in inventory as soon as game starts running
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMenuOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive || noticeActive) // checks all relevant game states to prevent unwanted player movement
        {
            PlayerController.instance.canMove = false; // sets canMove in player controller to false to prevent player movement
        }
        else
        {
            PlayerController.instance.canMove = true; // if no cases are true, sets canMove to true to allow player movement
        }

        /*
        // *DEBUG ONLY* - checks for J key press to test item add
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            AddItem("Iron Armor"); // adds Iron Armor to inventory
            AddItem("Dwarven Sword"); // adds Dwarven Sword to inventory
        }
        
        // *DEBUG ONLY* - checks for O button press for testing data save
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveData(); // calls save data function to save player scene and position
        }

        // *DEBUG ONLY* - checks for P button press for testing data load
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadData(); // calls load data function to load player scene and position
        }
        */
    }

    public Item GetItemDetails(string itemToGrab) // creates function to get item details from reference item array
    {
        for(int i=0; i< referenceItems.Length; i++) // increments through array of reference items
        {
            if (referenceItems[i].itemName == itemToGrab) // checks if element in reference item array matches name of itemToGrab
            {
                return referenceItems[i]; // returns referenceItem when a match is found
                                          // because function is of type Item, it returns an Item object
            }
        }
        return null; // returns null just to manage every possible outcome
    }

    public string GetItemTier(string itemName) // creates function to find tier of item based on name
    {
        //Debug.Log("Item name = " + itemName);
        Item item = GetItemDetails(itemName); // grabs details of passed item
        
        // checks item tier and formats string color appropriately
        if (item.tier == "Common")
        {
            itemName = "<color=white>" + item.itemName + "</color>";
        }
        else if (item.tier == "Magic")
        {
            itemName = "<color=#0089FFFF>" + item.itemName + "</color>";
        }
        else if (item.tier == "Rare")
        {
            itemName = "<color=yellow>" + item.itemName + "</color>";
        }
        else if (item.tier == "Legendary")
        {
            itemName = "<color=orange>" + item.itemName + "</color>";
        }
        else if (item.tier == "Unique")
        {
            itemName = "<color=magenta>" + item.itemName + "</color>";
        }

        return itemName;
    }

    public void SortItems() // creates function to sort items in inventory
    {
        bool itemAfterSpace = true; // creates bool variable to handle if an item is present after a space or empty item
                                    // is used to determine if the sorting function is complete or not
                                    // initialized to true to start the while loop
        while (itemAfterSpace) // executes loop as long as itemAfterSpace is true
        {
            itemAfterSpace = false; // sets item after space check to false

            for (int i = 0; i < itemsHeld.Length - 1; i++) // iterates through elements in itemsHeld array
            {
                if (itemsHeld[i] == "") // checks if element in itemsHeld array is empty
                {
                    itemsHeld[i] = itemsHeld[i + 1]; // copies next element to current element in itemsHeld array
                    itemsHeld[i + 1] = ""; // clears next element in itemsHeld array to prevent duplicates

                    numberOfItems[i] = numberOfItems[i + 1]; // copies next element to current element in numberOfItems array
                    numberOfItems[i + 1] = 0; // clears next element in numberOfItems array to prevent duplicates

                    if (itemsHeld[i] != "") // sets itemAfterSpace to true in case a real item was moved
                                            // this detects if the sorting run is complete
                    {
                        itemAfterSpace = true;
                    }
                }
            }     
        }
    }

    public void AddItem(string itemToAdd) // creates function to add item to inventory
    {
        int newItemPosition = 0; // creates an int to handle the position in the inventory while searching for an item

        bool foundSpace = false; // creates bool to handle if an item has been found at a location in the inventory

        for (int i = 0; i < itemsHeld.Length; i++) // iterates through all elements of the itemsHeld array
        {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd)             // checks if element in itemsHeld is blank or equal to the item name to add
            {
                newItemPosition = i; // sets new item position to current iteration

                i = itemsHeld.Length; // sets i to length of array to exit for loop

                foundSpace = true; // sets found space to true
            }
        }

        if (foundSpace) // checks if space was found while iterating through inventory
        {
            bool itemExists = false; // creates bool to handle check if item exists from the master item array

            for (int i = 0; i < referenceItems.Length; i++) // iterates through array of reference items
            {
                if (referenceItems[i].itemName == itemToAdd) // checks if reference item name at current iteration matches itemToAdd
                {
                    itemExists = true; // confirms that item exists

                    i = referenceItems.Length; // sets i to length of array to exit for loop
                }
            }

            if (itemExists) // executes if item to add exists
            {
                itemsHeld[newItemPosition] = itemToAdd; // adds item to add to inventory at this position

                numberOfItems[newItemPosition]++; // increments number of items at this position
            }
            else
            {
                Debug.LogError(itemToAdd + " doesn't exist!"); // outputs debug error if item does not exist
            }
        }

        GameMenu.instance.ShowItems(); // calls show items function to visually add new item in and update menu
    }

    public void RemoveItem(string itemToRemove) // creates function to remove item from inventory
    {
        // creates bool and int variables to manage search through item inventory
        bool foundItem = false;
        int itemPosition = 0;

        for(int i = 0; i < itemsHeld.Length; i++) // iterates through array of items held
        {
            if (itemsHeld[i] == itemToRemove) // checks if the item name at current index matches the item to remove
            {
                // sets variables to report item was found and location
                foundItem = true;
                itemPosition = i;

                i = itemsHeld.Length; // sets i to length of array to exit for loop
            }
        }

        if (foundItem) // executes if item was found
        {
            numberOfItems[itemPosition]--; // decrements number of items at required position in inventory

            if (numberOfItems[itemPosition] <= 0) // checks if number of items at position is <= 0 - no items left
            {
                itemsHeld[itemPosition] = ""; // sets itemsHeld string to blank
            }

            GameMenu.instance.ShowItems(); // calls ShowItems function to visually remove item and update menu
        }
        else
        {
            Debug.LogError("Couldn't find " + itemToRemove + "!"); // outputs debug error if item to remove can't be found
        }
    }

    public void SaveData(int saveSlot) // creates function to handle saving all game data
    {
        // saves player scene and position
        PlayerPrefs.SetString(saveSlot + "_Current_Scene", SceneManager.GetActiveScene().name); // pulls active scene name from scene manager, stores to player prefs
        PlayerPrefs.SetFloat(saveSlot + "_Player_Position_x", PlayerController.instance.transform.position.x); // pulls player x-position from player controller, stores to player prefs
        PlayerPrefs.SetFloat(saveSlot + "_Player_Position_y", PlayerController.instance.transform.position.y); // pulls player y-position from player controller, stores to player prefs
        PlayerPrefs.SetFloat(saveSlot + "_Player_Position_z", PlayerController.instance.transform.position.z); // pulls player z-position from player controller, stores to player prefs

        // saves active party list
        for (int i = 0; i < GameMenu.instance.activePartyList.Length; i++) // iterates through active party list
        {
            PlayerPrefs.SetString(saveSlot + "_ActiveParty_" + i, GameMenu.instance.activePartyList[i]); // saves each element of active party list to player prefs
        }
        // saves inactive party list
        for (int i = 0; i < GameMenu.instance.inactivePartyList.Length; i++) // iterates through inactive party list
        {
            PlayerPrefs.SetString(saveSlot + "_InactiveParty_" + i, GameMenu.instance.inactivePartyList[i]); // saves each element of inactive party list to player prefs
        }

        // saves character info and stats
        for (int i = 0; i < playerStats.Length; i++) // iterates through all player stats locations
        {
            if (playerStats[i].inFrontRow) // checks if player character is in the front row
            {
                PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_InFrontRow", 1); // saves player front row tag to player prefs
            }
            else // executes if player character is in the back row
            {
                PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_InFrontRow", 0); // saves player back row tag to player prefs
            }

            // saves all player stats to player prefs based on char name
            PlayerPrefs.SetString(saveSlot + "_Player_" + playerStats[i].charName + "_Status", playerStats[i].statusEffect);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentExp", playerStats[i].currentEXP);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_APLevel", playerStats[i].playerAPLevel);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentAP", playerStats[i].currentAP);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentSP", playerStats[i].currentSP);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_MaxSP", playerStats[i].maxSP);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Strength", playerStats[i].strength);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Tech", playerStats[i].tech);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Endurance", playerStats[i].endurance);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Agility", playerStats[i].agility);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Luck", playerStats[i].luck);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Speed", playerStats[i].speed);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_DmgWeapon", playerStats[i].dmgWeapon);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_HitChance", playerStats[i].hitChance);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CritWeapon", playerStats[i].critWeapon);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CritChance", playerStats[i].critChance);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_EvadeArmor", playerStats[i].evadeArmor);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_EvadeChance", playerStats[i].evadeChance);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_BlockShield", playerStats[i].blockShield);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_BlockChance", playerStats[i].blockChance);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_DefWeapon", playerStats[i].defWeapon);
            PlayerPrefs.SetInt(saveSlot + "_Player_" + playerStats[i].charName + "_DefTech", playerStats[i].defTech);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResHeat", playerStats[i].resistances[0]);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResFreeze", playerStats[i].resistances[1]);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResShock", playerStats[i].resistances[2]);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResVirus", playerStats[i].resistances[3]);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResChem", playerStats[i].resistances[4]);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResKinetic", playerStats[i].resistances[5]);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResWater", playerStats[i].resistances[6]);
            PlayerPrefs.SetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResQuantum", playerStats[i].resistances[7]);
            PlayerPrefs.SetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedWpn", playerStats[i].equippedWpn);
            PlayerPrefs.SetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedOff", playerStats[i].equippedOff);
            PlayerPrefs.SetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedArmr", playerStats[i].equippedArmr);
            PlayerPrefs.SetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedAccy", playerStats[i].equippedAccy);
        }

        // saves party leader name and level to player prefs
        for (int i = 0; i < GameMenu.instance.activePartyList.Length; i++) // iterates through active party list
        {
            if (GameMenu.instance.activePartyList[i] != "Empty") // checks if name in list is not empty
            {
                PlayerPrefs.SetString(saveSlot + "_LeaderName", GameMenu.instance.activePartyList[i]); // saves leader name
                PlayerPrefs.SetString(saveSlot + "_LeaderLV", GameMenu.instance.FindPlayerStats(GameMenu.instance.activePartyList[i]).playerLevel.ToString());  // saves leader level after finding player stats by name
                break; // breaks loop after first non-empty name is found
            }
        }

        // saves gold to player prefs
        PlayerPrefs.SetInt(saveSlot + "_CurrentGold", currentGold);

        // saves inventory data to player prefs based on array location
        for (int i = 0; i < itemsHeld.Length; i++) // iterates through all items held locations
        {
            PlayerPrefs.SetString(saveSlot + "_ItemInInventory_" + i, itemsHeld[i]);
            PlayerPrefs.SetInt(saveSlot + "_ItemAmount_" + i, numberOfItems[i]);
        }
        
        // saves tooltip option selection
        if (GameMenu.instance.showStatusTooltip.isOn)
        {
            PlayerPrefs.SetInt(saveSlot + "_ShowTooltips", 1);
        }
        else
        {
            PlayerPrefs.SetInt(saveSlot + "_ShowTooltips", 0);
        }

        // saves game timer
        PlayerPrefs.SetInt(saveSlot + "_Hours", GameMenu.instance.hourCount);
        PlayerPrefs.SetInt(saveSlot + "_Minutes", GameMenu.instance.minuteCount);
        PlayerPrefs.SetInt(saveSlot + "_Seconds", (int)GameMenu.instance.secondsCount);
    }

    public void LoadData(int saveSlot) // creates function to handle loading all game data
    {
        // pulls player position x-y-z from player prefs and loads to player controller transform position
        PlayerController.instance.transform.position = new Vector3(PlayerPrefs.GetFloat(saveSlot + "_Player_Position_x"), PlayerPrefs.GetFloat(saveSlot + "_Player_Position_y"), PlayerPrefs.GetFloat(saveSlot + "_Player_Position_z"));

        // loads active party list
        for (int i = 0; i < GameMenu.instance.activePartyList.Length; i++) // iterates through active party list
        {
            GameMenu.instance.activePartyList[i] = PlayerPrefs.GetString(saveSlot + "_ActiveParty_" + i); // loads each element of active party list from player prefs
        }
        // loads inactive party list
        for (int i = 0; i < GameMenu.instance.inactivePartyList.Length; i++) // iterates through inactive party list
        {
            GameMenu.instance.inactivePartyList[i] = PlayerPrefs.GetString(saveSlot + "_InactiveParty_" + i); // loads each element of inactive party list from player prefs
        }

        for (int i = 0; i < playerStats.Length; i++) // iterates through all player stats locations
        {
            if (PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_InFrontRow") == 0) // checks if player character is not in the front row
            {
                playerStats[i].inFrontRow = false; // sets player front row status to false
            }
            else // executes if player is in the front row
            {
                playerStats[i].inFrontRow = true; // sets player front row status to true
            }

            // loads all player stats from player prefs based on char name
            playerStats[i].statusEffect = PlayerPrefs.GetString(saveSlot + "_Player_" + playerStats[i].charName + "_Status");
            playerStats[i].playerLevel = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Level");
            playerStats[i].currentEXP = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentExp");
            playerStats[i].playerAPLevel = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_APLevel");
            playerStats[i].currentAP = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentAP");
            playerStats[i].currentHP = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentHP");
            playerStats[i].maxHP = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_MaxHP");
            playerStats[i].currentSP = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CurrentSP");
            playerStats[i].maxSP = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_MaxSP");
            playerStats[i].strength = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Strength");
            playerStats[i].tech = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Tech");
            playerStats[i].endurance = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Endurance");
            playerStats[i].agility = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Agility");
            playerStats[i].luck = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Luck");
            playerStats[i].speed = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_Speed");
            playerStats[i].dmgWeapon = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_DmgWeapon");
            playerStats[i].hitChance = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_HitChance");
            playerStats[i].critWeapon = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CritWeapon");
            playerStats[i].critChance = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_CritChance");
            playerStats[i].evadeArmor = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_EvadeArmor");
            playerStats[i].evadeChance = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_EvadeChance");
            playerStats[i].blockShield = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_BlockShield");
            playerStats[i].blockChance = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_BlockChance");
            playerStats[i].defWeapon = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_DefWeapon");
            playerStats[i].defTech = PlayerPrefs.GetInt(saveSlot + "_Player_" + playerStats[i].charName + "_DefTech");
            playerStats[i].resistances[0] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResHeat");
            playerStats[i].resistances[1] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResFreeze");
            playerStats[i].resistances[2] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResShock");
            playerStats[i].resistances[3] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResVirus");
            playerStats[i].resistances[4] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResChem");
            playerStats[i].resistances[5] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResKinetic");
            playerStats[i].resistances[6] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResWater");
            playerStats[i].resistances[7] = PlayerPrefs.GetFloat(saveSlot + "_Player_" + playerStats[i].charName + "_ResQuantum");
            playerStats[i].equippedWpn = PlayerPrefs.GetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedWpn");
            playerStats[i].equippedOff = PlayerPrefs.GetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedOff");
            playerStats[i].equippedArmr = PlayerPrefs.GetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedArmr");
            playerStats[i].equippedAccy = PlayerPrefs.GetString(saveSlot + "_Player_" + playerStats[i].charName + "_EquippedAccy");
        }

        // loads gold from player prefs
        currentGold = PlayerPrefs.GetInt(saveSlot + "_CurrentGold");

        // loads inventory data to player prefs based on array location
        for (int i = 0; i < itemsHeld.Length; i++) // iterates through all items held locations
        {
            itemsHeld[i] = PlayerPrefs.GetString(saveSlot + "_ItemInInventory_" + i);
            numberOfItems[i] = PlayerPrefs.GetInt(saveSlot + "_ItemAmount_" + i);
        }

        // loads tooltip option selection
        if (PlayerPrefs.GetInt(saveSlot + "_ShowTooltips") == 1)
        {
            GameMenu.instance.showStatusTooltip.isOn = true;
        }
        else
        {
            GameMenu.instance.showStatusTooltip.isOn = false;
        }

        // loads game timer
        GameMenu.instance.hourCount = PlayerPrefs.GetInt(saveSlot + "_Hours");
        GameMenu.instance.minuteCount = PlayerPrefs.GetInt(saveSlot + "_Minutes");
        GameMenu.instance.secondsCount = (float)PlayerPrefs.GetInt(saveSlot + "_Seconds");
    }

    public void RestoreHPSP() // creates function to fully restore party HP/SP
    {
        for(int i = 0; i < playerStats.Length; i++) // iterates through all players
        {
            playerStats[i].currentHP = playerStats[i].maxHP; // restores HP to max
            playerStats[i].currentSP = playerStats[i].maxSP; // restores SP to max
        }
    }
}