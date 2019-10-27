// AreaExit
// handles the player position and fade to black when exiting a scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class AreaExit : MonoBehaviour
{
    public string areaToLoad, areaTransitionName; // creates string variable to handle which area to load and transition name

    public AreaEntrance theEntrance; // creates reference to our AreaEntrance script

    public float waitToLoad = 1f; // creates float variable to handle delay time for scene load, which allows time for fade to/from black

    private bool shouldLoadAfterFade; // creates bool variable to handle logic for scene loading after fade to/from black

    // Start is called before the first frame update
    void Start()
    {
        theEntrance.transitionName = areaTransitionName; // sets the entrance transition name to the name of this area transition
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldLoadAfterFade) // checks if shouldLoadAfterFade is set

        {
            waitToLoad -= Time.deltaTime; // decrements waitToLoad value from initial value based on frame rate to ensure consistent actual time
                                          // result should be that wait time is equal to waitToLoad value in seconds
            if (waitToLoad <= 0) // checks if waitToLoad time has reached 0
            {
                shouldLoadAfterFade = false; //resets shouldLoadAfterFade to false since new scene is about to load
           
                SceneManager.LoadScene(areaToLoad); //calls SceneManager.LoadScene in order to load new scene
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // detects when player collides with the area exit to handle area transitions and UI fades
    {
        if(other.tag == "Player") // checks if it is the player that has collided with the area exit
        {
            shouldLoadAfterFade = true; // sets shouldLoadAfterFade in order to enable new scene load
            
            GameManager.instance.fadingBetweenAreas = true; // sets fadingBetweenAreas tag to true to prevent player movement

            UIFade.instance.FadeToBlack(); // calls FadeToBlack function in UIFade script to trigger fade

            PlayerController.instance.areaTransitionName = areaTransitionName; // passes areaTransitionName to same variable in PlayerController script
                                                                               // can access any variable attached to PlayerController script "instance" since it is static
        }
    }
}