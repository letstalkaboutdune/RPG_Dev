// LoadGame
// handles the display of load game screen from main menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    private bool sceneLoaded = false; // creates bool to manage if scene is loaded

    // Start is called before the first frame update
    void Awake()
    {
        sceneLoaded = true; // sets scene loaded bool to true
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneLoaded) // checks if sceneLoaded is true
        {
            LoadGameWindow(); // calls function to show load game window
        }
    }

    void LoadGameWindow() // creates function to show load game window
    {
        sceneLoaded = false;
        GameMenu.instance.theMenu.SetActive(true); // displays menu if not already active
        GameManager.instance.gameMenuOpen = true; // sets gameMenuOpen tag to true to prevent player movement
        GameMenu.instance.UpdateMainStats(); // updates stats in menu
        GameMenu.instance.ToggleWindow(3); // calls function to open save/load window
        GameMenu.instance.ShowSaveWindow(2); // calls function to open save/load window in load from main menu mode
    }
}
