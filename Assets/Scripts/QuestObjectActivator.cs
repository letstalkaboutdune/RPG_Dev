// QuestObjectActivator
// handles activation of quest objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectActivator : MonoBehaviour
{
    public GameObject objectToActivate; // creates game object reference of object to activate

    public string questToCheck; // creates string to handle quest name to check
    
    public bool activeIfComplete; // creates bool to handle if object should be active depending on quest completion

    private bool initialCheckDone; // creates bool to handle initial check of status to prevent execution order issues

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialCheckDone) // checks if initial check is false
        {
            initialCheckDone = true; // sets initial check status to true
            CheckCompletion(); // checks completion of questToCheck
        }
    }
    public void CheckCompletion() // creates function to check completion and handle object activation status
    {
        if (QuestManager.instance.CheckIfComplete(questToCheck)) // checks if quest is complete
        {
            objectToActivate.SetActive(activeIfComplete); // activates/deactivates game object depending on activeIfComplete setting
        }
    }
}
