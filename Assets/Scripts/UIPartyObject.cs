// UIPartyObject
// handles display of single active party members in UI

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPartyObject : MonoBehaviour, IPointerClickHandler
{
    // creates variables to manage needed data types for UI party object
    public PartyObject player;
    private Image playerImage;
    private UIPartyObject selectedPlayer;

    private void Awake()
    {
        playerImage = GetComponent<Image>(); // finds Image object associated with this script
        //Debug.Log("Found image = " + playerImage);
        UpdatePlayer(null); // tests script by passing an empty player on start
        selectedPlayer = GameObject.Find("Selected Player").GetComponent<UIPartyObject>(); // gets reference to selected player
    }

    public void UpdatePlayer(PartyObject player) // creates function to update active player slot
    {
        this.player = player; // assigns the passed player object to this player object

        if(this.player != null) // checks if player is not empty
        {
            playerImage.color = Color.white; // sets color to white and opaque
            playerImage.sprite = this.player.portrait; // sets sprite image to the player portrait
        }
        else // executes if player object is empty
        {
            playerImage.color = Color.clear; // sets color to clear to hide item
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.player != null) // checks if this player is not empty
        {
            if (selectedPlayer.player != null) // checks if previously selected player is not empty
            {
                PartyObject clone = new PartyObject(selectedPlayer.player); // creates a party object to store a copy of the selected player
                selectedPlayer.UpdatePlayer(this.player); // takes player we clicked on, puts it into selectedPlayer
                UpdatePlayer(clone); // saves dragging player inside the inventory
            }
            else // executes if previously selected player was empty
            {
                selectedPlayer.UpdatePlayer(this.player); // sets selected player to this player
                UpdatePlayer(null); // clears previous selected player slot
            }
        }
        else if (selectedPlayer.player != null) // executes if no player in inventory and a player is selected
        {
            UpdatePlayer(selectedPlayer.player); // drops this player in the inventory
            selectedPlayer.UpdatePlayer(null); // sets selected player to null
        }
    }
}
