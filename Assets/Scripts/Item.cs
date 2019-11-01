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
    public bool isAccy;
    public bool isRanged;

    [Header("Item Details")] // creates Item Details header in Unity

    // create string variables to handle item names and desciptions
    public string itemName;
    public string description;

    public int value; // create int variable to handle item shop value

    public Sprite itemSprite; // create Sprite variable to handle item image sprite

    [Header("Item Effects")] // creates Item Effects header in Unity

    // creates bool variables to manage type of item effect
    public bool affectHP;
    public bool affectMP;
    public bool affectStr;
    public bool affectLife;

    public int amountToChange; // create int variable to handle numerical amount of item effect

    [Header("Weapon/Armor/Accy Details")] // creates Weapon/Armor Details header in Unity

    // create int variables to handle weapon and armor strength
    public int dmgWeapon;
    public int defWeapon;
    public int defTech;
    
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

    
    public void Use(int charToUseOn) // creates function to handle item use
                                     // requires int of char index to use item on
    {
        //Debug.Log("Item.Use passed item = " + itemName); // prints passed item name to debug log

        CharStats selectedChar = GameManager.instance.playerStats[charToUseOn]; // creates new CharStats object which copies that location's contents of the player stats array
                       
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
                    /*
                    else if (BattleManager.instance.activeBattlers[charToUseOn].currentHP <= 0) // checks if player is dead
                    {
                        warningText = selectedChar.charName + " must be revived first!"; // sets dead player notification
                        ShowWarning(); // shows warning text
                    }
                    */
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
                    /*
                    else if (selectedChar.currentHP <= 0) // checks if player is dead
                    {
                        warningText = selectedChar.charName + " must be revived first!"; // sets dead player notification
                        ShowWarning(); // shows warning text
                    }
                    */
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

            else if (affectMP) // checks if item affects MP
            {
                if (BattleManager.instance.battleActive) // checks if a battle is active
                {
                    if (BattleManager.instance.activeBattlers[charToUseOn].currentMP == BattleManager.instance.activeBattlers[charToUseOn].maxMP) // checks if active battler current MP = max MP
                    {
                        warningText = selectedChar.charName + " already has maximum MP!"; // sets max MP error notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        BattleManager.instance.activeBattlers[charToUseOn].currentMP += amountToChange; // adds item value to battle char current MP

                        if (BattleManager.instance.activeBattlers[charToUseOn].currentMP > BattleManager.instance.activeBattlers[charToUseOn].maxMP) // checks if battle char current MP > max MP
                        {
                            BattleManager.instance.activeBattlers[charToUseOn].currentMP = BattleManager.instance.activeBattlers[charToUseOn].maxMP; // limits battle char current MP to max MP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
                else // executes if a battle is not active
                {
                    if (selectedChar.currentMP == selectedChar.maxMP) // checks if the player current MP = max MP
                    {
                        warningText = selectedChar.charName + " already has maximum MP!"; // sets max MP error notification
                        ShowWarning(); // shows warning text
                    }
                    else
                    {
                        // adds item value to selected char MP
                        selectedChar.currentMP += amountToChange;

                        if (selectedChar.currentMP > selectedChar.maxMP) // checks if char current MP has become larger than max MP
                        {
                            selectedChar.currentMP = selectedChar.maxMP; // limits char current MP to max MP
                        }

                        GameManager.instance.RemoveItem(itemName); // removes item from inventory
                    }
                }
            }

            else if (affectStr) // checks if item affects Strength
            {
                // adds item value to selected char and battle char Strength
                selectedChar.strength += amountToChange;

                if (GameManager.instance.battleActive)
                {
                    BattleManager.instance.activeBattlers[charToUseOn].strength += amountToChange;
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
            else // executes if same weapon is not already equipped
            {
                if (selectedChar.equippedWpn != "") // checks if equipped weapon slot on selected character is blank
                {
                    GameManager.instance.AddItem(selectedChar.equippedWpn); // adds previously equipped weapon to inventory
                }

                // enables equip effects of weapon on character
                selectedChar.equippedWpn = itemName;
                selectedChar.dmgWeapon = dmgWeapon;

                if (GameManager.instance.battleActive) // checks if battle is active
                {
                    BattleManager.instance.activeBattlers[charToUseOn].dmgWeapon = dmgWeapon; // enables equip effects of weapon on battle character
                }
            
                GameManager.instance.RemoveItem(itemName); // removes item from inventory
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
                    GameManager.instance.AddItem(selectedChar.equippedArmr); // adds previously equipped armor to inventory
                }
                
                // enables equip effects of armor on character
                selectedChar.equippedArmr = itemName;
                selectedChar.defWeapon = defWeapon;
                selectedChar.defTech = defTech;

                if (GameManager.instance.battleActive) // checks if battle is active
                {
                    // enables equip effects of armor on battle character
                    BattleManager.instance.activeBattlers[charToUseOn].defWeapon = defWeapon;
                    BattleManager.instance.activeBattlers[charToUseOn].defTech = defTech;
                }

                GameManager.instance.RemoveItem(itemName); // removes item from inventory
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
                    GameManager.instance.AddItem(selectedChar.equippedAccy); // adds previously equipped accessory to inventory
                }

                // enables equip effects of accessory on character
                selectedChar.equippedAccy = itemName;                
                
                for(int i = 0; i < resistances.Length; i++) // iterates through all elements of resistances array
                {
                    if (resistances[i] != 0f) // checks if resistance is not 0
                    {
                        selectedChar.resistances[i] = resistances[i]; // applies resistance to character
                    }
                }

                if (GameManager.instance.battleActive) // checks if battle is active
                {
                    // enables equip effects of accessory on battle character
                    for (int i = 0; i < resistances.Length; i++) // iterates through all elements of resistances array
                    {
                        if (resistances[i] != 0f) // checks if resistance is not 0
                        {
                            BattleManager.instance.activeBattlers[charToUseOn].resistances[i] = resistances[i]; // applies resistance to battle character
                        }
                    }
                }

                GameManager.instance.RemoveItem(itemName); // removes item from inventory
            }
        }
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