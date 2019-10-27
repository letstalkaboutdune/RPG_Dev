// MainMenu
// handles button execution on main menu screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library



public class MainMenu : MonoBehaviour
{
    public string newGameScene; // creates string to handle name of scene when starting new game

    public string loadGameScene; // creates string to handle name of load game scene

    public GameObject continueButton; // creates game object to handle activation of continue button

    public int soundToPlay; // creates int to handle button sound selection

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("Current_Scene")) // checks if player prefs has key for current scene, some game data has been saved
        {
            continueButton.SetActive(true); // sets continue button to active
        }
        else
        {
            continueButton.SetActive(false); // sets continue button to inactive
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Continue()
    {
        SceneManager.LoadScene(loadGameScene); // loads continued game at saved scene
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
