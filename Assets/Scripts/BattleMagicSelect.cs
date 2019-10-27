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
        if (BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP >= spellCost) // checks if player's MP is sufficient to cast the spell
        {        
            BattleManager.instance.magicMenu.SetActive(false); // hides magic menu
            BattleManager.instance.OpenTargetMenu(spellName); // calls open target menu to get targets for spells
            BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP -= spellCost; // subtracts MP from player to pay for spell
        }
        else // executes if player doesn't have enough MP to cast the spell
        {
            BattleManager.instance.battleNotice.theText.text = "Not enough MP!"; // sets battle notification text to MP warning
            BattleManager.instance.battleNotice.Activate(); // activates battle notification object
            BattleManager.instance.magicMenu.SetActive(false); // backs out of magic menu
        }

    }
}
