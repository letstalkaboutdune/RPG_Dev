// BattleMagicSelect
// handles player magic selection in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class BattleMagicSelect : MonoBehaviour
{
    // creates values to handle name, cost, and text UI for spells
    public string spellName;
    public int spellCost;
    public Text nameText;
    public Text costText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press()
    {
        bool spellFound = false; // creates int to store index of found spell, initializes to -1 by default
        
        for(int i = 0; i < BattleManager.instance.movesList.Length; i++) // iterates through entire move list
        {
            if(spellName == BattleManager.instance.movesList[i].moveName) // executes once spell is found in move list
            {
                spellFound = true;
            }
        }

        if (spellFound == true) // executes if spellFound is true, meaning the spell was found
        {
            if (BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentSP >= spellCost) // checks if player's SP is sufficient to cast the spell
            {
                BattleManager.instance.magicMenu.SetActive(false); // hides magic menu
                BattleManager.instance.OpenTargetMenu(spellName); // calls open target menu to get targets for spells
                BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentSP -= spellCost; // subtracts SP from player to pay for spell
            }
            else // executes if player doesn't have enough SP to cast the spell
            {
                BattleManager.instance.battleNotice.theText.text = "Not enough SP!"; // sets battle notification text to SP warning
                BattleManager.instance.battleNotice.Activate(); // activates battle notification object
                BattleManager.instance.magicMenu.SetActive(false); // backs out of magic menu
            }
        }
        else // executes if the spell was not found
        {
            BattleManager.instance.battleNotice.theText.text = "Ability not valid!"; // sets battle notification text to SP warning
            BattleManager.instance.battleNotice.Activate(); // activates battle notification object
            BattleManager.instance.magicMenu.SetActive(false); // backs out of magic menu
        }
    }
}
