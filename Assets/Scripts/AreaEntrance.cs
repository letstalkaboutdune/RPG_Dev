// AreaEntrance
// handles the player position and fade from black when entering a new scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntrance : MonoBehaviour
{

    public string transitionName; // creates string variable to handle scene transition name

    // Start is called before the first frame update
    void Start()
    {
        if(transitionName == PlayerController.instance.areaTransitionName) // checks if transition name on the player instance is equal to this script's transition name
        {
            PlayerController.instance.transform.position = transform.position; // sets the player position to the transform position of the entrance object
        }

        UIFade.instance.FadeFromBlack(); // calls FadeFromBlack function in UIFade script to trigger fade

        GameManager.instance.fadingBetweenAreas = false; // sets fadingBetweenAreas tag to false to prevent player movement
    }

    // Update is called once per frame
    void Update()
    {

    }
}