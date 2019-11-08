// LoadingScene
// handles the loading of different scenes and game/quest data

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class LoadingScene : MonoBehaviour
{
    public float waitToLoad; // creates float to handle time to wait for scene load

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(waitToLoad > 0) // checks if wait to load time is non-zero
        {
            waitToLoad -= Time.deltaTime; // decrements wait to load time based on frame rate
            
            if(waitToLoad <= 0) // executes once wait to load time reaches zero
            {
                SceneManager.LoadScene(PlayerPrefs.GetString("Current_Scene")); // loads saved scene from player prefs

                // loads game data and quest data
                GameManager.instance.LoadData();
                QuestManager.instance.LoadQuestData();
            }
        }
    }
}
