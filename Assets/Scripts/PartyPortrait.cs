using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartyPortrait : MonoBehaviour, IPointerClickHandler
{
    private Image playerImage;
    public Image selectedPlayer;

    private void Awake()
    {
        playerImage = GetComponent<Image>(); // finds Image associated with this script
        selectedPlayer.color = Color.clear; // nulls selected player sprite

        // prints image found notices to debug log
        //Debug.Log("Found image = " + playerImage); // prints image found to debug log
        //Debug.Log("Selected player = " + selectedPlayer);
    }

    // executes when an object with this script is clicked
    public void OnPointerClick(PointerEventData eventData) 
    {
        if (playerImage.color != Color.clear) // checks if player slot is visible
        {
            if(selectedPlayer.color != Color.clear) // checks if selected player is visible
            {
                PartyObject clone = new PartyObject(selectedPlayer.sprite); // clones selected player to store for image swap
                selectedPlayer.sprite = playerImage.sprite; // sets selected player sprite to player slot sprite
                playerImage.sprite = clone.portrait; // sets player slot sprite to cloned sprite
            }
            else // executes if selected item was empty
            {
                selectedPlayer.sprite = playerImage.sprite; // sets selected player sprite to player slot sprite
                selectedPlayer.color = Color.white; // sets selected player to opaque
                playerImage.color = Color.clear; // sets player slot to clear
            }
        }
        else if (selectedPlayer.color != Color.clear) // executes if no player in slot and player selected
        {
            playerImage.sprite = selectedPlayer.sprite; // sets player slot sprite to selected player sprite
            playerImage.color = Color.white; // sets player slot to opaque
            selectedPlayer.color = Color.clear; // sets player slot to clear
        }
    }

}
