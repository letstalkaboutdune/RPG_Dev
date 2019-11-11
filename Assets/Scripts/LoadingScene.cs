// LoadingScene
// handles the loading of different scenes and game/quest data

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class LoadingScene : MonoBehaviour
{
    public float waitToLoad; // creates float to handle time to wait for scene load
    private bool loadMainMenu = false; // creates bool to handle loading main menu, default of false

    // Start is called before the first frame update
    void Start()
    {
        // pulls flag for whether to load main menu 
        loadMainMenu = GameMenu.instance.loadMainMenu;
        //Debug.Log("loadMainMenu = " + loadMainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if(waitToLoad > 0) // checks if wait to load time is non-zero
        {
            waitToLoad -= Time.deltaTime; // decrements wait to load time based on frame rate
            
            if(waitToLoad <= 0) // executes once wait to load time reaches zero
            {
                if (loadMainMenu) // checks if load main menu flag is set
                {
                   //Debug.Log("Loading main menu.");
                    
                    // destroys any open objects from currently open scene
                    Destroy(PlayerController.instance.gameObject);
                    Destroy(GameManager.instance.gameObject);
                    Destroy(AudioManager.instance.gameObject);
                    Destroy(GameMenu.instance.gameObject);
                    
                    SceneManager.LoadScene("MainMenu"); // loads main menu                    
                }
                else // executes if neither load main menu flag is not set
                {
                    //Debug.Log("Loading game.");
                    SceneManager.LoadScene(PlayerPrefs.GetString(GameMenu.instance.slotToLoad + "_Current_Scene")); // loads saved scene from player prefs

                    // loads game data and quest data
                    GameManager.instance.LoadData(GameMenu.instance.slotToLoad);
                    QuestManager.instance.LoadQuestData(GameMenu.instance.slotToLoad);
                    GameMenu.instance.CloseMenu();
                }
            }
        }
    }    
}
