// MainMenu
// handles button execution on main menu screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class MainMenu : MonoBehaviour
{
    public string loadGameScene, newGameScene; // creates string to handle name of load game and new game scenes

    public GameObject loadButton; // creates game object to handle load button

    public int soundToPlay; // creates int to handle button sound selection

    // Start is called before the first frame update
    void Start()
    {
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
        SceneManager.LoadScene(loadGameScene); // loads load game scene
    }

    public void NewGame() // creates function to load new game at first scene
    {
        SceneManager.LoadScene(newGameScene); // loads new game at new game scene
    }

    public void Exit() // creates function to exit game
    {
        Application.Quit(); // exits game
    }

    public void PlayButtonSound(int soundToPlay) // creates function to play UI beeps, requires int of sound to play
    {
        AudioManager.instance.PlaySFX(soundToPlay); //; plays UI beep sound from audio manager
    }
}
