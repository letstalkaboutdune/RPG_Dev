// PartyDatabase
// handles building of party database from GameManager data

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyDatabase : MonoBehaviour
{
    public List<PartyObject> partyMembers = new List<PartyObject>(); // creates a list of PartyObjects to act as our item database

    private void Awake()
    {
        BuildDatabase(); // calls function to build database
    }

    public PartyObject GetPartyObject(string charName) // creates a function to return a PartyObject by passing its name
    {
        return partyMembers.Find(player => player.name == charName); // uses built-in Find function to find partyobject by name
    }

    void BuildDatabase() // creates function to build party database
    {
        // creates local variables to manage data pulled from 
        CharStats stats;
        string name;
        int order;

        // creates a new list of type PartyObject
        partyMembers = new List<PartyObject>();
                   
        // iterates through all elements of playerStats array to build party object database
        for (int i = 0; i < GameManager.instance.playerStats.Length; i++)
        {
            // pulls needed stats from GameManager
            stats = GameManager.instance.playerStats[i];
            name = GameManager.instance.playerStats[i].charName;
            order = GameManager.instance.playerStats[i].partyOrder;
            partyMembers.Add(new PartyObject(stats, name, order)); // adds new PartyObject to database
        }
    }
}
