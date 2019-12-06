// MainMenu
// handles button execution on main menu screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class MainMenu : MonoBehaviour
{
    //public string loadGameScene, newGameScene; // creates string to handle name of load game and new game scenes

    public GameObject loadButton; // creates game object to handle load button

    public int soundToPlay; // creates int to handle button sound selection

    public GameObject mainMenu; // creates game object to handle main menu UI

    private GameObject player; // creates reference to player

    public static MainMenu instance; // creates reference to this instance of the main menu script

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets instance to this instance

        GameManager.instance.gameMenuOpen = true; // sets gameMenuOpen flag true to stop player movement

        // grabs reference to player controller and sets inactive
        player = GameObject.Find("Player(Clone)");
        player.SetActive(false);

        if (SceneManager.GetActiveScene().name != "MainMenu") // checks if current scene is not the main menu
        {
            HideMainMenu(); // calls function to hide main menu
        }

        for (int i = 0; i < 3; i++) // iterates through all 3 possible player prefs slots
        {
            if (PlayerPrefs.HasKey(i + "_Current_Scene")) // checks if player prefs has key for current scene, some game data has been saved
            {
                loadButton.SetActive(true); // sets continue button to active
                break; // break loop as soon as an active save slot is found
            }
            else
            {
                loadButton.SetActive(false); // sets continue button to inactive
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    public void LoadGame()
    {
        GameMenu.instance.loadLoadGame = true;
        SceneManager.LoadScene("LoadingScene"); // loads load game scene
    }

    public void NewGame() // creates function to load new game at first scene
    {
        GameMenu.instance.loadNewGame = true;
        SceneManager.LoadScene("LoadingScene"); // loads new game at new game scene
    }

    public void ExitGame() // creates function to exit game
    {
        Application.Quit(); // exits game        
    }

    public void PlayButtonSound(int soundToPlay) // creates function to play UI beeps, requires int of sound to play
    {
        AudioManager.instance.PlaySFX(soundToPlay); //; plays UI beep sound from audio manager
    }

    public void HideMainMenu() // creates function to hide main menu UI
    {
        if(GameMenu.instance != null)
        {
            mainMenu.SetActive(false); // hides main menu UI
        }

        if(GameManager.instance != null)
        {
            GameManager.instance.gameMenuOpen = false; // sets gameMenuOpen flag false to allow player movement        
        }

        if(player != null)
        {
            player.SetActive(true); // shows player
        }
    }
}
