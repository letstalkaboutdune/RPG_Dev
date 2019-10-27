// QuestMarker
// handles quest marking and quest object interaction

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMarker : MonoBehaviour
{
    public string questToMark; // creates string to manage associated quest name
    public bool markComplete; // creates bool to handle if quest is marked complete
    
    public bool markOnEnter; // creates bool to handle if quest should be marked immediately on collider enter
    private bool canMark; // creates bool to handle if player can interact with and mark the quest object
    public bool deactivateOnMarking; // creates bool to handle if associated game object should be deactivated upon marking

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canMark && Input.GetButtonDown("Fire1")) // checks if player can mark and is clicking Fire1
        {
            canMark = false; // resets canMark as it is no longer needed
            MarkQuest(); // calls mark quest function
        }
    }

    public void MarkQuest() // creates function to mark quests
    {
        if (markComplete) // checks if quest should be marked complete
        {
            QuestManager.instance.MarkQuestComplete(questToMark); // marks quest as complete using function
        }
        else
        {
            QuestManager.instance.MarkQuestIncomplete(questToMark); // marks quest as incomplete using function
        }

        gameObject.SetActive(!deactivateOnMarking); // activates/deactivates game object based on setting
    }

    private void OnTriggerEnter2D(Collider2D other) // runs when object enters linked collider
    {
        if(other.tag == "Player") // checks if object that collided is the player
        {
            if (markOnEnter) // checks if player can mark quest on enter
            {
                MarkQuest(); // calls mark quest function associated with object
            }
            else
            {
                canMark = true; // sets canMark flag to true to enable manipulation
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) // runs when object exits linked collider
    {
        if (other.tag == "Player") // checks if object that collided is the player
        {
            canMark = false; // sets canMark flag to false to disable manipulation
        }
    }
}
