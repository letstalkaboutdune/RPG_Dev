// GameOver
// handles the control of game over scene when party dies in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // includes Unity SceneManagement library

public class GameOver : MonoBehaviour
{
    public string mainMenuScene, loadGameScene; // creates strings to handle scene loads out of game over screen

    public int soundToPlay; // creates int to handle button sound selection

    public GameObject loadGameButton; // creates game object to control state of load last save button

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBGM(4); // calls audio manager to play game over music

        PlayerController.instance.gameObject.SetActive(false); // deactivates player controll to prevent unwanted action

        for (int i = 0; i < 3; i++) // iterates 3 times (number of save slots)
        {
            if (PlayerPrefs.HasKey(i + "_Current_Scene")) // checks if player prefs has key for current scene, some game data has been saved
            {
                //Debug.Log("Save slot found."); // prints save slot found notice to debug log
                loadGameButton.SetActive(true); // sets load game button to active
                break; // breaks loop once a save slot is found
            }
            else // executes if no save slot has data
            {
                //Debug.Log("No save slot found."); // prints save slot not found notice to debug log
                loadGameButton.SetActive(false); // sets load game button to inactive
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitToMain() // creates function to handle quit to main menu
    {
        // destroys all loaded essential objects
        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);

        SceneManager.LoadScene(mainMenuScene); // calls scene manager to load main menu
    }

    public void LoadGame() // creates function to handle loading load game screen
    {
        // destroys all loaded essential objects
        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
        //Destroy(AudioManager.instance.gameObject); // preserves audio manager
        Destroy(BattleManager.instance.gameObject);

        SceneManager.LoadScene(loadGameScene); // calls scene manager to load load game scene
    }

    public void PlayButtonSound(int soundToPlay) // creates function to play UI beeps, requires int of sound to play
    {
        AudioManager.instance.PlaySFX(soundToPlay); //; plays UI beep sound from audio manager
    }
}
