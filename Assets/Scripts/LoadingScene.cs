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
    private bool loadNewGame = false; // creates bool to handle loading new game, default of false
    private bool loadLoadGame = false; // creates bool to handle loading load game, default of false

    // Start is called before the first frame update
    void Start()
    {
        // pulls flags for whether to load main menu, new game, or load game 
        loadMainMenu = GameMenu.instance.loadMainMenu;
        loadNewGame = GameMenu.instance.loadNewGame;
        loadLoadGame = GameMenu.instance.loadLoadGame;
        //Debug.Log("loadMainMenu = " + loadMainMenu);

        MainMenu.instance.HideMainMenu(); // calls function to hide main menu
    }

    // Update is called once per frame
    void Update()
    {
        if(waitToLoad > 0) // checks if wait to load time is non-zero
        {
            waitToLoad -= Time.deltaTime; // decrements wait to load time based on frame rate
            
            if(waitToLoad <= 0) // executes once wait to load time reaches zero
            {
                // WIP
                if (loadMainMenu || loadNewGame || loadLoadGame) // checks if any load flags are set
                {                    
                    ClearLoadingFlags(); // calls function to clear loading scene flags

                    if (loadMainMenu) // executes if load main menu flag is set
                    {
                        //Debug.Log("Loading main menu.");
                        DestroyOpenObjects(); // calls function to destroy open objects
                        SceneManager.LoadScene("MainMenu"); // loads main menu                    

                    }
                    else if (loadNewGame) // executes if load new game flag is set
                    {
                        SceneManager.LoadScene("Countryside"); // loads countryside scene
                        GameMenu.instance.CloseMenu();
                    }
                    else // executes if load load game flag is set
                    {
                        SceneManager.LoadScene("LoadGame"); // loads load game scene
                    }
                }
                // END WIP

                else // executes if no load flags are set
                {
                    //Debug.Log("Loading game.");
                    
                    // WIP
                    GameMenu.instance.loadYesNoButtons.SetActive(false); // hides load yes/no buttons
                    GameMenu.instance.gameNotification.SetActive(false); // hides game notification
                    // END WIP
                    
                    SceneManager.LoadScene(PlayerPrefs.GetString(GameMenu.instance.slotToLoad + "_Current_Scene")); // loads saved scene from player prefs
                    PlayerController.instance.CheckIfWorldMap(); // calls function to check if scene is a world map

                    // loads game data and quest data
                    GameManager.instance.LoadData(GameMenu.instance.slotToLoad);
                    QuestManager.instance.LoadQuestData(GameMenu.instance.slotToLoad);
                    GameMenu.instance.CloseMenu();
                }
            }
        }
    }

    public void ClearLoadingFlags() // creates function to clear all loading scene flags
    {
        // clears all loading flags
        GameMenu.instance.loadMainMenu = false;
        GameMenu.instance.loadNewGame = false;
        GameMenu.instance.loadLoadGame = false;
    }

    public void DestroyOpenObjects() // creates function to destroy open objects from scene
    {
        // destroys any open objects from currently open scene                    
        Destroy(PlayerController.instance.gameObject);
        Destroy(GameManager.instance.gameObject);
        //Destroy(AudioManager.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
    }
}
