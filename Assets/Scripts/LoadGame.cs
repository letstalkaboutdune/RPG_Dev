// LoadGame
// handles the display of load game screen from main menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadGameWindow(); // calls function to show load game window
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LoadGameWindow() // creates function to show load game window
    {
        GameMenu.instance.theMenu.SetActive(true); // displays menu if not already active
        GameMenu.instance.UpdateMainStats(); // updates stats in menu
        GameMenu.instance.ToggleWindow(3); // calls function to open save/load window
        GameMenu.instance.ShowSaveWindow(2); // calls function to open save/load window in load from main menu mode
        GameManager.instance.gameMenuOpen = true; // sets gameMenuOpen tag to true to prevent player movement
    }
}
