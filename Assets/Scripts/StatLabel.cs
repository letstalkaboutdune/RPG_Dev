// StatLabel
// handles the mouseover detection to show stat tooltips

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatLabel : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    private Tooltip tooltip; // creates reference to tooltip to control tooltip display

    // Start is called before the first frame update
    private void Awake()
    {
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>(); // gets reference to tooltip
    }

    // function is called by IPointerEnterHandler whenever a user mouses over an item
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameMenu.instance.showStatusTooltip.isOn)
        {
            tooltip.GenerateTooltip(gameObject.name); // calls function to generate and display tooltip based on name of stat label
        }
    }

    // function is called by IPointerExitHandler whenever a user stops mousing over a stat label
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false); // hides tooltip
    }
}
