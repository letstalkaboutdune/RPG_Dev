// UIPartyActive
// handles display of active party members in UI

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartyActive : MonoBehaviour
{
    // creates references to required variables and data types
    public List<UIPartyObject> uiPlayers = new List<UIPartyObject>();
    public GameObject slotPrefab;
    public Transform slotPanel;
    public int numberOfSlots = 3;

    private void Awake()
    {
        for(int i = 0; i < numberOfSlots; i++) // iterates through all slots
        {
            GameObject instance = Instantiate(slotPrefab); // instantiates slot prefab
            instance.transform.SetParent(slotPanel); // sets slot as child of slot panel
            uiPlayers.Add(instance.GetComponentInChildren<UIPartyObject>()); // adds slot to uiPlayers list
        }
    }

    public void UpdateSlot(int slot, PartyObject player) // creates function to show/hide items
    {
        uiPlayers[slot].UpdatePlayer(player); // calls function in UIPartyObject to update player in passed slot
    }

    public void AddNewPlayer(PartyObject player) // creates function to add new player to first available slot
    {
        // WIP
        for(int i = 0; i < numberOfSlots; i++) // iterates through all available slots
        {
            if(uiPlayers[i] == null) // finds if slot is empty
            {
                UpdateSlot(i, player); // calls update slot function at index when empty slot is found
                break; // breaks loop when empty slot is found
            }
        }
        // END WIP

        //UpdateSlot(uiPlayers.FindIndex(i => i.player == null), player); // finds first slot where there is no player, then passes player there with update slot
    }

    public void RemovePlayer(PartyObject player) // creates function to remove player from inventory
    {
        UpdateSlot(uiPlayers.FindIndex(i => i.player == player), null); // replaces passed player with null
    }
}
