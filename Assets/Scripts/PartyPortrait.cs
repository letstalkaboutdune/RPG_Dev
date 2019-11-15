// PartyPortrait
// handles drag-and-drop interface of portraits on party menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartyPortrait : MonoBehaviour, IPointerClickHandler
{
    // assigns needed variables for party portrait class
    public Image selectedPlayer;
    private Image playerImage;
    private Sprite emptySprite;

    private void Awake()
    {
        emptySprite = Resources.Load<Sprite>("Sprites/Portraits/Empty"); // assigns empty portrait to empty sprite
        playerImage = GetComponent<Image>(); // finds Image associated with this script
        selectedPlayer.sprite = emptySprite; // initializes selected player sprite to empty portrait

        // prints image found notices to debug log
        //Debug.Log("Found image = " + playerImage); // prints image found to debug log
        //Debug.Log("Selected player = " + selectedPlayer);
    }

    private void Update() // executes once per frame
    {
        if(selectedPlayer.sprite.name == "Empty") // checks if selected player is empty
        {
            GameMenu.instance.playerSelected = false; // sets player selected to false to allow party menu open/close
        }
        else // executes otherwise
        {
            GameMenu.instance.playerSelected = true; // sets player selected to true to prevent party menu open/close
        }
    }

    // executes when an object with this script is clicked
    public void OnPointerClick(PointerEventData eventData) 
    {
        if (playerImage.sprite != emptySprite) // checks if player slot sprite is not empty
        {
            if (selectedPlayer.sprite != emptySprite) // checks if select player sprite is not empty
            {
                PartyObject clone = new PartyObject(selectedPlayer.sprite); // clones selected player to store for image swap
                selectedPlayer.sprite = playerImage.sprite; // sets selected player sprite to player slot sprite
                playerImage.sprite = clone.portrait; // sets player slot sprite to cloned sprite
                GameMenu.instance.UpdatePartyOrder(); // calls function to update party order
            }
            else // executes if selected player sprite is empty
            {
                selectedPlayer.sprite = playerImage.sprite; // sets selected player sprite to player slot sprite
                playerImage.sprite = emptySprite; // sets player slot sprite to empty
            }
        }
        else if (selectedPlayer.sprite != emptySprite) // executes if no player in slot and player selected
        {
            playerImage.sprite = selectedPlayer.sprite; // sets player slot sprite to selected player sprite
            selectedPlayer.sprite = emptySprite; //  sets selected player sprite to empty
            GameMenu.instance.UpdatePartyOrder(); // calls function to update party order
        }
    }



}
