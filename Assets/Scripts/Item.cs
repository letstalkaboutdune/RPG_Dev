// Item
// handles the defition of all items and use of an item

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Type")]  // creates Item Type header in Unity

    // create bool variables to handle item type
    public bool isItem;
    public bool isWeapon;
    public bool isArmor;
    public bool isOffHand;
    public bool isAccy;

    [Header("Item Tags")] // creates Item Tags header in Unity
    
    // creates bool variables to handle item tags
    public bool isRanged;
    public bool isTwoHanded;
    public bool isShield;

    [Header("Item Details")] // creates Item Details header in Unity

    // create string variables to handle item names and desciptions
    public string itemName;
    public string description;
    public string tier;
    public int value; // create int variable to handle item shop value

    public Sprite itemSprite; // create Sprite variable to handle item image sprite

    [Header("Item Effects")] // creates Item Effects header in Unity

    // creates bool variables to manage type of item effect
    public bool affectHP;
    public bool affectSP;
    public bool affectLife;

    public int amountToChange; // create int variable to handle numerical amount of item effect

    [Header("Weapon/Armor/Accy Details")] // creates Weapon/Armor Details header in Unity

    // create int variables to handle equippable item properties
    public int dmgWeapon;
    public int defWeapon;
    public int defTech;
    public int evadeArmor;
    public int blockShield;
    public int hitChance;
    public int critWeapon;
    
    public float[] resistances = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f }; // creates array of floats to handle elemental resistances and initializes to default of 0
    // [0] = heat
    // [1] = freeze
    // [2] = shock
    // [3] = virus
    // [4] = chem
    // [5] = kinetic
    // [6] = water
    // [7] = quantum

    public string warningText; // creates string to handle error/warning text

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Use(string charName) // creates function to handle item use
                                     // requires int of char index to use item on
    {
        //Debug.Log("Item.Use passed item = " + itemName); // prints passed item name to debug log

        CharStats selectedChar = GameMenu.instance.FindPlayerStats(charName); // pulls reference to selected player stats by name
        int charToUseOn = BattleManager.instance.FindPlayerBattlerIndex(charName); // pulls reference to active battler player index by name
        int charToEquip = GameMenu.instance.FindPlayerIndex(charName); // pulls character index in player stats array by name
                       
        if (isItem) // checks if item is of type item
        {
            if (affectLife) // checks if item affects Life status
            {
                if (BattleManager.instance.battleActive) // checks if a battle is active
                {
                    if (BattleManager.instance.activeBattlers[charToUseOn].currentHP > 0) // checks if player is alive
                    {
                        warningText = selectedChar.charName + " is already alive!"; // sets max HP error notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        BattleManager.instance.activeBattlers[charToUseOn].currentHP += amountToChange; // adds item value to battle char current HP

                        if (BattleManager.instance.activeBattlers[charToUseOn].currentHP > BattleManager.instance.activeBattlers[charToUseOn].maxHP) // checks if battle char current HP > max HP
                        {
                            BattleManager.instance.activeBattlers[charToUseOn].currentHP = BattleManager.instance.activeBattlers[charToUseOn].maxHP; // limits battle char current HP to max HP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
                else // executes if a battle is not active
                {
                    if (selectedChar.currentHP > 0) // checks if player is alive
                    {
                        warningText = selectedChar.charName + " is already alive!"; // sets max HP error notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        // adds item value to selected char HP
                        selectedChar.currentHP += amountToChange;

                        if (selectedChar.currentHP > selectedChar.maxHP) // checks if char current HP has become larger than max HP
                        {
                            selectedChar.currentHP = selectedChar.maxHP; // limits char current HP to max HP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
            }

            else if (affectHP) // checks if item affects HP
            {
                if (BattleManager.instance.battleActive) // checks if a battle is active
                {
                    if (BattleManager.instance.activeBattlers[charToUseOn].currentHP == BattleManager.instance.activeBattlers[charToUseOn].maxHP) // checks if active battler current HP = max HP
                    {
                        warningText = selectedChar.charName + " already has maximum HP!"; // sets max HP error notification
                        ShowWarning(); // shows warning text
                    }                   
                    else if (BattleManager.instance.activeBattlers[charToUseOn].currentHP <= 0) // checks if player is dead
                    {
                        warningText = selectedChar.charName + " must be revived first!"; // sets dead player notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        BattleManager.instance.activeBattlers[charToUseOn].currentHP += amountToChange; // adds item value to battle char current HP

                        if (BattleManager.instance.activeBattlers[charToUseOn].currentHP > BattleManager.instance.activeBattlers[charToUseOn].maxHP) // checks if battle char current HP > max HP
                        {
                            BattleManager.instance.activeBattlers[charToUseOn].currentHP = BattleManager.instance.activeBattlers[charToUseOn].maxHP; // limits battle char current HP to max HP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
                else // executes if a battle is not active
                {
                    if (selectedChar.currentHP == selectedChar.maxHP) // checks if the player current HP = max HP
                    {
                        warningText = selectedChar.charName + " already has maximum HP!"; // sets max HP error notification
                        ShowWarning(); // shows warning text
                    }
                    else if (selectedChar.currentHP <= 0) // checks if player is dead
                    {
                        warningText = selectedChar.charName + " must be revived first!"; // sets dead player notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        // adds item value to selected char HP
                        selectedChar.currentHP += amountToChange;

                        if (selectedChar.currentHP > selectedChar.maxHP) // checks if char current HP has become larger than max HP
                        {
                            selectedChar.currentHP = selectedChar.maxHP; // limits char current HP to max HP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
            }

            else if (affectSP) // checks if item affects SP
            {
                if (BattleManager.instance.battleActive) // checks if a battle is active
                {
                    if (BattleManager.instance.activeBattlers[charToUseOn].currentSP == BattleManager.instance.activeBattlers[charToUseOn].maxSP) // checks if active battler current SP = max SP
                    {
                        warningText = selectedChar.charName + " already has maximum SP!"; // sets max SP error notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        BattleManager.instance.activeBattlers[charToUseOn].currentSP += amountToChange; // adds item value to battle char current SP

                        if (BattleManager.instance.activeBattlers[charToUseOn].currentSP > BattleManager.instance.activeBattlers[charToUseOn].maxSP) // checks if battle char current SP > max SP
                        {
                            BattleManager.instance.activeBattlers[charToUseOn].currentSP = BattleManager.instance.activeBattlers[charToUseOn].maxSP; // limits battle char current SP to max SP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
                else // executes if a battle is not active
                {
                    if (selectedChar.currentSP == selectedChar.maxSP) // checks if the player current SP = max SP
                    {
                        warningText = selectedChar.charName + " already has maximum SP!"; // sets max SP error notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        // adds item value to selected char SP
                        selectedChar.currentSP += amountToChange;

                        if (selectedChar.currentSP > selectedChar.maxSP) // checks if char current SP has become larger than max SP
                        {
                            selectedChar.currentSP = selectedChar.maxSP; // limits char current SP to max SP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
            }
        }    

        else if (isWeapon) // checks if item is of type weapon
        {
            if(selectedChar.equippedWpn == itemName) // checks if same weapon is already equipped
            {
                // displays error message, does not use item
                warningText = selectedChar.charName + " already has " + itemName + " equipped!"; // gives equip weapon warning
                ShowWarning(); // shows warning text
            }

            // NEED TO UPDATE CHARTOUSEON REFERENCES WITH CHARTOEQUIP
            else // executes if same weapon is not already equipped
            {
                if (selectedChar.equippedWpn != "") // checks if equipped weapon slot is not blank
                {
                    Unequip(selectedChar.equippedWpn, charToEquip); // calls function to unequip current weapon

                    if(selectedChar.equippedOff != "" && isTwoHanded) // checks if equipped offhand slot is not blank and weapon to equip is two-handed
                    {
                        Unequip(selectedChar.equippedOff, charToEquip); // calls function to unequip current offhand
                    }
                }
                else if (selectedChar.equippedOff != "") // checks if equipped weapon is blank but offhand is not blank
                {
                    if (isTwoHanded) // checks if item to equip is two-handed
                    {
                        Unequip(selectedChar.equippedOff, charToEquip); // calls function to unequip current offhand
                    }
                }

                Equip(itemName, charToEquip); // calls function to equip new weapon

                selectedChar.CalculateDerivedStats(); // re-calculates new derived stats on character after equipping new weapon
            }         
        }

        else if (isOffHand) // checks if item is of type offhand
        {
            if (selectedChar.equippedOff == itemName) // checks if same offhand item is already equipped
            {
                // displays error message, does not use item
                warningText = selectedChar.charName + " already has " + itemName + " equipped!"; // gives equip offhand warning
                ShowWarning(); // shows warning text
            }
            else // executes if same offhand is not already equipped
            {
                if(selectedChar.equippedWpn != "" && GameManager.instance.GetItemDetails(selectedChar.equippedWpn).isTwoHanded) // checks if equipped weapon is not blank and is two-handed
                {
                    Unequip(selectedChar.equippedWpn, charToEquip); // calls function to unequip current weapon
                }
                else if (selectedChar.equippedOff != "") // checks if equipped offhand is not blank
                {
                    Unequip(selectedChar.equippedOff, charToEquip); // calls function to unequip current offhand
                }

                Equip(itemName, charToEquip); // calls function to equip new offhand

                selectedChar.CalculateDerivedStats(); // re-calculates new derived stats on character after equipping new offhand
            }
        }

        else if (isArmor) // checks if item is of type armor
        {
            if (selectedChar.equippedArmr == itemName) // checks if same armor is already equipped
            {
                // displays error message, does not use item
                warningText = selectedChar.charName + " already has " + itemName + " equipped!"; // gives equip armor warning
                ShowWarning(); // shows warning text
            }
            else // executes if same armor is not already equipped
            {
                if (selectedChar.equippedArmr != "") // checks if equipped armor slot on selected character is blank
                {
                    Unequip(selectedChar.equippedArmr, charToEquip); // calls function to unequip current armor
                }                

                Equip(itemName, charToEquip); // calls function to equip new offhand

                selectedChar.CalculateDerivedStats(); // re-calculates new derived stats on character after equipping new armor
            }
        }
        
        else if (isAccy) // checks if item is of type accessory
        {
            if (selectedChar.equippedAccy == itemName) // checks if same accessory is already equipped
            {
                // displays error message, does not use item
                warningText = selectedChar.charName + " already has " + itemName + " equipped!"; // gives equip accessory warning
                ShowWarning(); // shows warning text
            }
            else // executes if same accessory is not already equipped
            {
                if (selectedChar.equippedAccy != "") // checks if equipped accessory slot on selected character is blank
                {
                    Unequip(selectedChar.equippedAccy, charToEquip); // calls function to unequip current accessory
                }

                Equip(itemName, charToEquip); // calls function to equip new accessory

                selectedChar.CalculateDerivedStats(); // re-calculates new derived stats on character after equipping new accessory
            }
        }        
    }

    public void Equip(string itemToEquip, int charToEquip) // creates function to handle equip of items
    {
        // pulls unequip item properties and selected character stats
        Item equipItem = GameManager.instance.GetItemDetails(itemToEquip); // pulls item details from passed item name
        CharStats selectedChar = GameManager.instance.playerStats[charToEquip]; // creates CharStats object which references the selected playerStats
        string charName = selectedChar.charName; // pulls reference to char name
        int charToUseOn = BattleManager.instance.FindPlayerBattlerIndex(charName); // pulls reference to active battler player index by name

        // applies all item stat boosts
        selectedChar.dmgWeapon += equipItem.dmgWeapon;
        selectedChar.critWeapon += equipItem.critWeapon;
        selectedChar.defWeapon += equipItem.defWeapon;
        selectedChar.defTech += equipItem.defTech;
        selectedChar.evadeArmor += equipItem.evadeArmor;
        selectedChar.blockShield += equipItem.blockShield;
        
        if (isWeapon) // checks if equipped item is weapon
        {
            selectedChar.hitChance = equipItem.hitChance; // sets char hit chance equal to item hit chance
        }

        // *** NEED TO HANDLE BASE STATS (ONCE IMPLEMENTED) ***

        // applies all item resistances
        for (int i = 0; i < equipItem.resistances.Length; i++) // iterates through all elements of item resistances array
        {
            if (equipItem.resistances[i] != 0f) // checks if resistance is not 0, meaning item affects that resistance
            {
                selectedChar.resistances[i] = equipItem.resistances[i]; // sets character resistance equal to item resistance
            }
        }

        // adds item to appropriate equip slots
        if (equipItem.isWeapon) // checks if item is a weapon
        {
            selectedChar.equippedWpn = itemToEquip; // sets item name to weapon equip slot

            if (equipItem.isTwoHanded) // checks if weapon is two handed
            {
                selectedChar.equippedOff = itemToEquip; // sets item name to offhand equip slot
            }
        }
        else if (equipItem.isArmor) // checks if item is an armor
        {
            selectedChar.equippedArmr = itemToEquip; // sets item name to armor equip slot
        }
        else if (equipItem.isOffHand) // checks if item is an offhand
        {
            selectedChar.equippedOff = itemToEquip; // sets item name to offhand equip slot
        }
        else if (equipItem.isAccy) // checks if item is an accessory
        {
            selectedChar.equippedAccy = itemToEquip; // sets item name to accessory equip slot
        }

        if (BattleManager.instance.battleActive) // checks if battle is active
        {
            // applies all item stat boosts to active battler
            BattleManager.instance.activeBattlers[charToUseOn].dmgWeapon += equipItem.dmgWeapon;
            BattleManager.instance.activeBattlers[charToUseOn].critWeapon += equipItem.critWeapon;
            BattleManager.instance.activeBattlers[charToUseOn].defWeapon += equipItem.defWeapon;
            BattleManager.instance.activeBattlers[charToUseOn].defTech += equipItem.defTech;
            BattleManager.instance.activeBattlers[charToUseOn].evadeArmor += equipItem.evadeArmor;
            BattleManager.instance.activeBattlers[charToUseOn].blockShield += equipItem.blockShield;

            if (isWeapon) // checks if equipped item is weapon
            {
                BattleManager.instance.activeBattlers[charToUseOn].hitChance = equipItem.hitChance; // sets active battler hit chance equal to item hit chance
            }

            // *** NEED TO HANDLE BASE STATS (ONCE IMPLEMENTED) ***

            // applies all item resistances to active battler
            for (int i = 0; i < equipItem.resistances.Length; i++) // iterates through all elements of item resistances array
            {
                if (equipItem.resistances[i] != 0f) // checks if resistance is not 0, meaning item affects that resistance
                {
                    BattleManager.instance.activeBattlers[charToUseOn].resistances[i] = equipItem.resistances[i]; // sets character resistance equal to item resistance
                }
            }

            // adds item to appropriate equip slots
            if (equipItem.isWeapon) // checks if item is a weapon
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedWpn = itemToEquip; // sets item name to weapon equip slot

                if (equipItem.isTwoHanded) // checks if weapon is two handed
                {
                    BattleManager.instance.activeBattlers[charToUseOn].equippedOff = itemToEquip; // sets item name to offhand equip slot
                }
            }
            else if (equipItem.isArmor) // checks if item is an armor
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedArmr = itemToEquip; // sets item name to armor equip slot
            }
            else if (equipItem.isOffHand) // checks if item is an offhand
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedOff = itemToEquip; // sets item name to offhand equip slot
            }
            else if (equipItem.isAccy) // checks if item is an accessory
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedAccy = itemToEquip; // sets item name to accessory equip slot
            }
        }

        GameManager.instance.RemoveItem(equipItem.name); // removes equipped item from inventory
    }

    public void Unequip(string itemToUnequip, int charToUnequip) // creates function to handle un-equip of items
    {
        // pull unequip item properties and selected character stats
        Item unequipItem = GameManager.instance.GetItemDetails(itemToUnequip); // pulls item details from passed item name
        CharStats selectedChar = GameManager.instance.playerStats[charToUnequip]; // creates CharStats object which references the selected playerStats
        string charName = selectedChar.charName; // pulls reference to char name
        int charToUseOn = BattleManager.instance.FindPlayerBattlerIndex(charName); // pulls reference to active battler player index by name

        // remove all item stat boosts and resistances
        selectedChar.dmgWeapon -= unequipItem.dmgWeapon;
        selectedChar.critWeapon -= unequipItem.critWeapon;
        selectedChar.defWeapon -= unequipItem.defWeapon;
        selectedChar.defTech -= unequipItem.defTech;
        selectedChar.evadeArmor -= unequipItem.evadeArmor;
        selectedChar.blockShield -= unequipItem.blockShield;

        if (isWeapon) // checks if unequipped item is weapon
        {
            selectedChar.hitChance = 90; // resets hit chance to default of 90%
        }

        // *** NEED TO HANDLE BASE STATS (ONCE IMPLEMENTED) ***
        // *** NEED TO HANDLE IF MULTIPLE ITEMS AFFECT SAME STATS/RESISTANCES ***
        // *** RE-EQUIP ALL OTHER ITEMS AFTER AN UNEQUIP? ***
        
        // removes item resistance effects
        for (int i = 0; i < unequipItem.resistances.Length; i++) // iterates through all elements of item resistances array
        {
            if(unequipItem.resistances[i] != 0f) // checks if resistance is not 0, meaning item affects that resistance
            {
                selectedChar.resistances[i] = selectedChar.baseResistances[i]; // sets character resistance to base resistance, removing item effect
            }
        }

        // removes item from appropriate equip slots
        if (unequipItem.isWeapon) // checks if item is a weapon
        {
            selectedChar.equippedWpn = ""; // sets weapon equip slot to blank

            if (unequipItem.isTwoHanded) // checks if weapon is two handed
            {
                selectedChar.equippedOff = ""; // sets offhand equip slot to blank
            }
        }
        else if (unequipItem.isArmor) // checks if item is an armor
        {
            selectedChar.equippedArmr = ""; // sets armor equip slot to blank
        }
        else if (unequipItem.isOffHand) // checks if item is an offhand
        {
            selectedChar.equippedOff = ""; // sets offhand equip slot to blank
        }
        else if (unequipItem.isAccy) // checks if item is an accessory
        {
            selectedChar.equippedAccy = ""; // sets accessory equip slot to blank
        }

        if (BattleManager.instance.battleActive) // checks if battle is active
        {
            // removes all item stat boosts from active battler
            BattleManager.instance.activeBattlers[charToUseOn].dmgWeapon -= unequipItem.dmgWeapon;
            BattleManager.instance.activeBattlers[charToUseOn].critWeapon -= unequipItem.critWeapon;
            BattleManager.instance.activeBattlers[charToUseOn].defWeapon -= unequipItem.defWeapon;
            BattleManager.instance.activeBattlers[charToUseOn].defTech -= unequipItem.defTech;
            BattleManager.instance.activeBattlers[charToUseOn].evadeArmor -= unequipItem.evadeArmor;
            BattleManager.instance.activeBattlers[charToUseOn].blockShield -= unequipItem.blockShield;

            if (isWeapon) // checks if equipped item is weapon
            {
                BattleManager.instance.activeBattlers[charToUseOn].hitChance = 90; // resets active battler hit chance equal to default of 90%
            }

            // *** NEED TO HANDLE BASE STATS (ONCE IMPLEMENTED) ***
            // *** NEED TO HANDLE IF MULTIPLE ITEMS AFFECT SAME STATS/RESISTANCES ***
            // *** RE-EQUIP ALL OTHER ITEMS AFTER AN UNEQUIP? ***

            // removes all item resistances from active battler
            for (int i = 0; i < unequipItem.resistances.Length; i++) // iterates through all elements of item resistances array
            {
                if (unequipItem.resistances[i] != 0f) // checks if resistance is not 0, meaning item affects that resistance
                {
                    BattleManager.instance.activeBattlers[charToUseOn].resistances[i] = selectedChar.baseResistances[i]; // sets character resistance to base resistance, removing item effect
                }
            }

            // removes item from appropriate equip slots
            if (unequipItem.isWeapon) // checks if item is a weapon
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedWpn = ""; // sets weapon equip slot to blank

                if (unequipItem.isTwoHanded) // checks if weapon is two handed
                {
                    BattleManager.instance.activeBattlers[charToUseOn].equippedOff = ""; // sets offhand equip slot to blank
                }
            }
            else if (unequipItem.isArmor) // checks if item is an armor
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedArmr = ""; // sets armor equip slot to blank
            }
            else if (unequipItem.isOffHand) // checks if item is an offhand
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedOff = ""; // sets offhand equip slot to blank
            }
            else if (unequipItem.isAccy) // checks if item is an accessory
            {
                BattleManager.instance.activeBattlers[charToUseOn].equippedAccy = ""; // sets accessory equip slot to blank
            }
        }

        GameManager.instance.AddItem(unequipItem.name); // adds unequipped item back into inventory
    }

    public void ShowWarning() // creates function to show warning text
    {
        if (BattleManager.instance.battleActive) // checks if battle is active
        {
            BattleManager.instance.battleNotice.theText.text = warningText; // sets warning text in battle menu
            BattleManager.instance.battleNoticeActive = true; // shows warning text in battle menu
        }
        else // executes if battle is not active
        {
            GameMenu.instance.itemNotificationText.text = warningText; // sets warning text in item menu
            GameMenu.instance.itemNoticeActive = true; // shows warning text in item menu
        }
    }

}