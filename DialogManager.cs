// DialogManager
// handles the execution of dialog, dialog-related quest tracking, and control of player movement during dialog 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class DialogManager : MonoBehaviour
{
    public Text dialogText, nameText; // creates Text variables for dialog test and name text in dialog UI
    public GameObject dialogBox, nameBox; // creates GameObject variables for dialog box and name box in dialog UI
    
    public string[] dialogLines; // creates array of strings to handle lines of dialog
    public int currentLine; // creates int to handle current line of dialog in the dialogLines array

    public bool justStarted; // creates bool to handle whether the dialog box has just started

    public static DialogManager instance; // creates static instance of DialogManager class/script

    // creates various variables to handle quest marking with dialog
    private string questToMark; 
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets the instance of DialogManager to this instance
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogBox.activeInHierarchy) // checks if dialog box is active in the scene
        {
            if (Input.GetButtonUp("Fire1")) // checks if player has pressed and released the Fire1 button
            {
                if (!justStarted) // checks if the dialog box has just been started
                {
                    currentLine++; // increments current line of dialog
                    
                    if (currentLine >= dialogLines.Length) // checks if currentLine is outside of array size to prevent out-of-bounds errors
                    {
                        dialogBox.SetActive(false); // de-activates dialogBox from the scene using SetActive function

                        GameManager.instance.dialogActive = false; // sets dialogActive tag to false to allow player movement

                        // PlayerController.instance.canMove = true; // *disabled* resets canMove to true to re-enable player movement after dialog

                        if (shouldMarkQuest) // checks if shouldMarkQuest is true to manage quest marking
                        {
                            shouldMarkQuest = false; // resets marker to prevent unwanted marking
                            if (markQuestComplete) // checks if quest should be marked complete
                            {
                                QuestManager.instance.MarkQuestComplete(questToMark); // calls mark quest complete function
                            }
                            else
                            {
                                QuestManager.instance.MarkQuestIncomplete(questToMark); // calls mark quest incomplete function
                            }
                        }
                    }
                    else
                    {
                        CheckIfName(); // calls CheckIfName function to handle name display instead of dialog display

                        dialogText.text = dialogLines[currentLine]; // displays the current line in the dialogLines array to the dialog text box
                    }
                }
                else
                {
                    justStarted = false; // resets justStarted to false
                }
            }
        }
    }


    public void ShowDialog(string[] newLines, bool isPerson, bool isAuto) // creates function to show dialog on screen that's accessible by DialogActivator script
                                                                          // requires an array of strings in order to execute
                                                                          // requires boolean to check if dialog source is a person or is automatically triggered
    {
        if (!GameManager.instance.noticeActive) // checks if notice dialog is not active to prevent additional dialog from opening
        {
            dialogLines = newLines; // assigns dialogLines to string array passed into function

            currentLine = 0; // resets current line to 0

            CheckIfName(); // resets current line to 0

            dialogText.text = dialogLines[currentLine]; // sets the dialog box text to the first string in the array

            dialogBox.SetActive(true); // activates the dialog box UI element

            if (!isAuto) // checks if dialog source is not automatically triggered
            {
                justStarted = true; // sets the justStarted bool to true to handle first-line display issues
            }

            nameBox.SetActive(isPerson); // activates the name box UI element

            GameManager.instance.dialogActive = true; // sets dialogActive tag to true to prevent player movement
        }      
    }

    public void CheckIfName()     // creates function to check if the current line in the dialog is a name
    {
        if (dialogLines[currentLine].StartsWith("n-")) // checks if current line of text starts with 'n-', our tag for a name
        {
            nameText.text = dialogLines[currentLine].Replace("n-", ""); // displays current line to the name text box
                                                                        // replaces 'n-' tag with blank
            currentLine++; // increments current line of dialog
        }
    }

    public void ShouldActivateQuestAtEnd(string questName, bool markComplete) // creates function to handle if dialog should mark quest upon completion
    {
        questToMark = questName; // assigns questToMark string to passed string
        markQuestComplete = markComplete; // assigns markQuestComplete bool to passed bool
        shouldMarkQuest = true; // sets shouldMarkQuest to true
    }
}