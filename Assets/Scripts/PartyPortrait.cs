// PartyPortrait
// handles drag-and-drop interface of portraits on party menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartyPortrait : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // assigns needed variables for party portrait class
    public Image selectedPlayer;
    private Image playerImage;
    private Sprite emptySprite;
    private Tooltip tooltip;

    private void Awake()
    {
        emptySprite = Resources.Load<Sprite>("Sprites/Portraits/Empty"); // assigns empty portrait to empty sprite
        playerImage = GetComponent<Image>(); // finds Image associated with this script
        selectedPlayer.sprite = emptySprite; // initializes selected player sprite to empty portrait
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>(); // gets reference to tooltip

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
            tooltip.gameObject.SetActive(false); // hides tooltip
           
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(playerImage.sprite.name != "Empty" && selectedPlayer.sprite.name == "Empty") // checks if sprite portrait is not empty and selected player is empty
        {
            //Debug.Log("playerImage sprite name = " + playerImage.sprite.name); // prints player name notice to debug log
            CharStats player = GameMenu.instance.FindPlayerStats(playerImage.sprite.name); // pulls reference to player stats based on sprite name
            //Debug.Log("Generating tooltip for " + player.charName); // prints tooltip notice to debug log
            tooltip.GenerateTooltip(player); // calls function to generate tooltip based on player stats
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Hiding tooltip"); // prints tooltip hide notice to debug log
        tooltip.gameObject.SetActive(false); // hides tooltip
    }
}
