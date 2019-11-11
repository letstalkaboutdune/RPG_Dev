// PartyActive
// handles control of active party members, like an inventory

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyActive : MonoBehaviour
{
    // creates new variables to manage organization/display of party objects
    public List<PartyObject> activeParty = new List<PartyObject>();
    public PartyDatabase partyDatabase;
    public UIPartyActive activePartyUI;

    private void Start() 
    {
        /*
        // *DISABLED* - BREAKS FUNCTION?
        // SEEMS TO BE ISSUES WITH "FIND" FUNCTION TO DETERMINE SLOT
        AddPlayer("Tim"); // calls function to add Tim to active party
        AddPlayer("Woody"); // calls function to add Woody to active party
        AddPlayer("Sleepy"); // calls function to add Woody to active party   
        RemovePlayer("Tim"); // calls function to remove Tim from active party
        */

        // WIP
        CharStats[] playerStats = GameManager.instance.playerStats; // pulls reference player stats
        for (int i = 0; i < playerStats.Length; i++) // iterates through all players
        {
            if (playerStats[i].gameObject.activeInHierarchy) // checks if player is active in hierarchy
            {
                AddPlayer(playerStats[i].charName); // adds player to active player list
            }
        }
        // END WIP
    }

    public PartyObject CheckForPlayer(string charName) // creates function to check if player is in active party list
    {
        return activeParty.Find(item => item.name == charName); // uses built-in Find function to find partyobject by name
    }

    public void AddPlayer(string charName) // creates a function to add player to active party
    {
        PartyObject playerToAdd = partyDatabase.GetPartyObject(charName); // calls get party object function to pull details
        activeParty.Add(playerToAdd); // adds party object to list
        activePartyUI.AddNewPlayer(playerToAdd); // adds party object to UI
        Debug.Log(charName + " added to party."); // prints player add notice to debug log
    }

    public void RemovePlayer(string charName) // creates function to remove player from active party
    {
        PartyObject playerToRemove = CheckForPlayer(charName); // calls function to check for player
        
        if (playerToRemove != null) // checks if player exists in active party list
        {
            activeParty.Remove(playerToRemove); // removes player from active party list
            activePartyUI.RemovePlayer(playerToRemove); // adds party object to UI
            Debug.Log(charName + " removed from party."); // prints player remove notice to debug log
        }
    }
}
