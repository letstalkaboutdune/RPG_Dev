// QuestManager
// handles quest data save/load, quest checking and marking, and manipulation of local quest objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string[] questMarkerNames; // creates string array to handle quest names
    public bool[] questMarkersComplete; // creates bool array to handle if certain stages or markers of quests are complete
    public static QuestManager instance; // creates public static instance of QuestManager
    public int displayTime; // creates int to handle time to display reward notification

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // assigns current instance of QuestManager to this

        questMarkersComplete = new bool[questMarkerNames.Length]; // sets questMarkersComplete length to same length as questMarkerNames
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // *DISABLED*
        // *DEBUG ONLY* - checks for Q button press for testing quest marking
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Debug.Log(CheckIfComplete("quest test")); // force test certain quest name string, print to debug log
            MarkQuestComplete("quest test"); // force test mark quest complete function
            MarkQuestIncomplete("fight the demon"); // Force test mark quest incomplete function
        }

        // *DEBUG ONLY* - checks for O button press for testing quest save
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveQuestData(); // calls save quest data function
        }

        // *DEBUG ONLY* - checks for O button press for testing quest load
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadQuestData(); // calls load quest data function
        }
        */
    }

    public int GetQuestNumber(string questToFind) // creates function to find quest marker number for certain quest name
    {
        for (int i = 0; i < questMarkerNames.Length; i++) // iterates through all elements of questMarkerNames array
        {
            if (questMarkerNames[i] == questToFind) // checks if current element matches questToFind
            {
                return i; // returns index of array
            }
        }

        Debug.LogError("Quest " + questToFind + " does not exist!"); // outputs debug error if quest number is not found
        return 0; // sets default return statement
    }

    public bool CheckIfComplete(string questToCheck) // creates function to check if quest marker is complete
    {
        if (GetQuestNumber(questToCheck) != 0) // checks if quest number of questToCheck is not equal to zero (invalid)
        {
            return questMarkersComplete[GetQuestNumber(questToCheck)]; // returns value in boolean array of complete quest markers at index of questToCheck
        }
        return false; // sets default return statement
    }

    public void MarkQuestComplete(string questToMark) // creates function to mark quest complete
    {
        questMarkersComplete[GetQuestNumber(questToMark)] = true; // gets index of questToMark in names array, sets boolean array at index to true
        UpdateLocalQuestObjects(); // updates status of local quest objects based on new markers

        switch (questToMark) // switch detects one case out of multiple possibilities
        {
            case "Town_gold_chest": // executes when Town_gold_chest quest marked complete
                Debug.Log("Awarded 100 gold."); // prints notice to debug log
                GameMenu.instance.notificationText.text = "Received 100 gold!"; // sets item notification text to show Dwarven Sword
                GameManager.instance.currentGold += 100; // adds 100 gold reward to inventory
                StartCoroutine(ShowGameNotification(1)); // calls game notification coroutine to show notice and block dialog
                break; // ends case

            case "Town_end": // executes when Town_end quest marked complete

                Debug.Log("Awarded Dwarven Sword."); // prints notice to debug log                
                GameMenu.instance.notificationText.text = "Received a " + GameManager.instance.GetItemTier("Dwarven Sword") + "!"; // sets item notification text to show Dwarven Sword
                GameManager.instance.AddItem("Dwarven Sword"); // adds Dwarven Sword reward to inventory
                StartCoroutine(ShowGameNotification(3)); // calls game notification coroutine to show notice and block dialog
                break; // ends case

            default: // default case executes if no other matches found
                Debug.Log("No quest rewards found."); // prints no match notice to debug log
                break; // ends case
        } 
    }

    public void MarkQuestIncomplete(string questToMark) // creates function to mark quest incomplete
    {
        questMarkersComplete[GetQuestNumber(questToMark)] = false; // gets index of questToMark in names array, sets boolean array at index to false
        UpdateLocalQuestObjects(); // updates status of local quest objects based on new markers
    }

    public void UpdateLocalQuestObjects() // creates function to update status of local quest objects
    {
        QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>(); // finds all objects in the scene with QuestObjectActivator script attached

        if (questObjects.Length > 0) // checks if any quest objects are present in the scene
        {
            for (int i = 0; i < questObjects.Length; i++) // iterates through all elements of scene quest objects array
            {
                questObjects[i].CheckCompletion(); // checks completion of all quest objects
            }
        }
    }

    public void SaveQuestData() // creates function to save quest data
    {
        for(int i = 0; i < questMarkerNames.Length; i++) // iterates through all quest marker names
        {
            if (questMarkersComplete[i]) // checks if quest markers in array are complete
            {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 1); // uses built-in Unity tool for PlayerPrefs to save quest data
                                                                             // only supports int, float, or string, so using int 1 or 0 as a bool
            }
            else // executes if quest markers in array are incomplete
            {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 0); // uses built-in Unity tool for PlayerPrefs to load quest data
            }
        }
    }

    public void LoadQuestData() // creates function to load quest data
    {
        for(int i = 0; i < questMarkerNames.Length; i++) // iterates through all quest marker names
        {
            int valueToSet = 0; // assumes we have a 0 default value for any quest marker names that are unset
            if(PlayerPrefs.HasKey("QuestMarker_" + questMarkerNames[i])) // checks if key in player prefs exists for each quest marker name
            {
                valueToSet = PlayerPrefs.GetInt("QuestMarker_" + questMarkerNames[i]); // writes any available quest marker key to valueToSet
            }

            if (valueToSet == 0) // checks if valueToSet is zero, meaning no quest marker saved
            {
                questMarkersComplete[i] = false; // writes quest marker in that location to false
            } 
            else // executes if valueToSet is non-zero, meaning quest marker saved
            {
                questMarkersComplete[i] = true; // writes quest marker in that location to true
            }
        }
    }

    public IEnumerator ShowGameNotification(int displayTime) // creates IEnumerator coroutine to show quest reward notification
    {
        GameManager.instance.noticeActive = true; // sets dialogActive true to stop player action
        GameMenu.instance.gameNotification.SetActive(true); // shows game notification panel

        yield return new WaitForSeconds(displayTime); // forces wait for reward notification to display

        GameMenu.instance.gameNotification.SetActive(false); // hides game notification panel      
        GameManager.instance.noticeActive = false; // sets dialogActive false to allow player action
    }
}
