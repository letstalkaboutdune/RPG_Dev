// AreaExit
// handles the player position and fade to black when exiting a scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library
using UnityEngine.UI;

public class AreaExit : MonoBehaviour
{
    public string areaToLoad, areaTransitionName; // creates string variable to handle which area to load and transition name

    public AreaEntrance theEntrance; // creates reference to our AreaEntrance script

    public float waitToLoad = 1f; // creates float variable to handle delay time for scene load, which allows time for fade to/from black

    private bool shouldLoadAfterFade; // creates bool variable to handle logic for scene loading after fade to/from black
    public bool activateOnEnter; // creates bool to handle if exit should occur when entering the collider
    private bool canActivate; // creates bool to check if player is in the correct area to activate the exit

    [Header("World Map")] // creates World Map header
    // creates variables to manage area details
    public GameObject areaDetails;
    public Text areaName;
    public Text areaCompletion;
    public string[] sceneQuests;

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

        if (canActivate && Input.GetButtonDown("Fire1")) // checks if collider is active and player clicks
        {
            //Debug.Log("Loading scene on player click.");

            ExitFade(); // calls exit fade function
        }
        else if (canActivate && activateOnEnter) // checks if collider is active and exit should occur automatically
        {
            activateOnEnter = false; // disables activate on enter flag to prevent duplicate loads

            //Debug.Log("Loading scene on entry.");

            ExitFade(); // calls exit fade function
        }        
    }

    private void OnTriggerEnter2D(Collider2D other) // detects when player enters collider of area exit
    {
        if (other.tag == "Player") // checks if tag of object that touched collider is the player
        {
            canActivate = true; // sets bool false to enable activation
            //Debug.Log("canActivate = " + canActivate);  

            if (PlayerController.instance.isWorldMap) // checks if scene is a world map
            {
                areaDetails.SetActive(true); // shows area details panel
                areaName.text = gameObject.name.Replace("Area Exit - ", "");  // sets area name text based on area exit game object name

                string completeString; // initializes string variable for quest complete number
                int completion = CalculateCompletion(); // calls function to calculate quest completion %
                
                if(completion == 100) // checks if all quests in scene are complete
                {
                    completeString = "<color=#F4B913FF>" + completion + "%</color>"; // builds, formats complete number in gold
                }
                else
                {
                    completeString = "<color=white>" + completion + "%</color>"; // builds, formats complete number in white
                }
                areaCompletion.text = "Completion: " + completeString; // builds completion % text
            }

        }
    }

    private void OnTriggerExit2D(Collider2D other) // detects when player leaves collider of area exit
    {
        if (other.tag == "Player") // checks if tag of object that touched collider is the player
        {
            canActivate = false; // sets bool false to disable activation
            //Debug.Log("canActivate = " + canActivate);

            if (PlayerController.instance.isWorldMap) // checks if scene is a world map
            {
                areaDetails.SetActive(false); // hides area details panel
            }
        }
    }

    public void ExitFade() // creates function to handle fade to black on scene exit
    {
        shouldLoadAfterFade = true; // sets shouldLoadAfterFade in order to enable new scene load
        GameManager.instance.fadingBetweenAreas = true; // sets fadingBetweenAreas tag to true to prevent player movement
        UIFade.instance.FadeToBlack(); // calls FadeToBlack function in UIFade script to trigger fade
        PlayerController.instance.areaTransitionName = areaTransitionName; // passes areaTransitionName to same variable in PlayerController script
                                                                           // can access any variable attached to PlayerController script "instance" since it is static
    }

    public int CalculateCompletion() // creates function to calculate % completion of a scene
    {
        bool[] questsComplete = new bool[sceneQuests.Length]; // initiates local bool[] to contain completion status of scene quests
        float completionTotal = 0f;
        int completionPercent;

        for (int i = 0; i < sceneQuests.Length; i++) // iterates through all associated scene quests
        {
            questsComplete[i] = QuestManager.instance.CheckIfComplete(sceneQuests[i]); // sets quest complete array element appropriately
            
            if (questsComplete[i] == true) // checks if quest is complete
            {
                completionTotal++; // increments completion total
            }
        }

        completionPercent = Mathf.RoundToInt((completionTotal / sceneQuests.Length) * 100f); // calculates completion percent

        //Debug.Log("Quests complete = " + completionTotal);
        //Debug.Log("Quests total = " + sceneQuests.Length);
        //Debug.Log("Completion percent = " + completionPercent + "%");
        
        return completionPercent; // returns completion percent
    }
}