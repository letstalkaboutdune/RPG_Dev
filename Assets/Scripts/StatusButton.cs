// StatusButton
// handles detection of click events on player status buttons in status menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) // executes when status button is clicked
    {
        string name = gameObject.GetComponentInChildren<Text>().text; // finds name on clicked button
        //Debug.Log("Name on button = " + name); // prints name to debug log

        int index = GameMenu.instance.FindPlayerIndex(name); // calls function to find player index based on name on button
        //Debug.Log("Index for " + name + " = " + index); // prints index to debug log

        GameMenu.instance.StatusChar(index); // calls function to update status based on found player index
    }
}
