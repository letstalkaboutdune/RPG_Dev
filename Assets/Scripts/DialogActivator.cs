// DialogActivator
// handles activation of dialog and dialog's effects on quests

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : MonoBehaviour
{
    public string[] lines; // creates array of strings to store lines of dialog

    private bool canActivate; // creates bool to check if the player is in the correct area to activate dialog

    public bool activateOnEnter; // creates bool to check if dialog should occur automatically

    public bool isPerson = true; // creates bool to check if dialog source is person and needs a name box
                                 // sets isPerson to true by default

    public bool isAuto = false; // creates bool to check if dialog source is automatically activated

    public bool shouldActivateQuest; // creates bool to check if quest should be activate
    public string questToMark; // creates bool to handle marked quest name
    public bool markComplete; // creates bool to handle if quest should be marked complete

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // checks if player is colliding with NPC, pressing the Fire1 button, and various game conditions are met
        if (canActivate && Input.GetButtonDown("Fire1") && !DialogManager.instance.dialogBox.activeInHierarchy && !GameManager.instance.gameMenuOpen && !GameManager.instance.fadingBetweenAreas && !GameManager.instance.shopActive && !GameManager.instance.battleActive && !GameManager.instance.noticeActive) 
        {
            DialogManager.instance.ShowDialog(lines, isPerson, isAuto); // calls ShowDialog function in DialogManager script and passes in lines array and isPerson boolean check
            DialogManager.instance.ShouldActivateQuestAtEnd(questToMark, markComplete); // calls function to manage quest marking
        }
        else if (canActivate && activateOnEnter) // occurs if dialog can activate and auto-activate on enter is true, to handle one-time scripted dialog
        {
            activateOnEnter = false; // disables activate on enter flag
            
            DialogManager.instance.justStarted = false; // sets justStarted flag to false to prevent double-click issues
            DialogManager.instance.ShowDialog(lines, isPerson, isAuto); // calls ShowDialog function in DialogManager script and passes in lines array and isPerson boolean check
            DialogManager.instance.ShouldActivateQuestAtEnd(questToMark, markComplete); // calls function to manage quest marking
            gameObject.SetActive(false); // de-activates dialog object once complete
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // OnTriggerEnter2D detects when player collides with the NPC to handle dialog box activation
    {
        if (other.tag == "Player") // checks if tag of object that touched collider is the player
        {
            canActivate = true; // sets boolean for enabling the dialog box
        }
    }

    private void OnTriggerExit2D(Collider2D other) // OnTriggerEnter2D detects when player no long collides with the NPC to handle dialog box de-activation
    {
        if (other.tag == "Player") // checks if tag of object that touched collider is the player
        {
            canActivate = false; // sets boolean for enabling the dialog box
        }
    }
}