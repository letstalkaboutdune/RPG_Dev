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

    public GameObject loadLastSaveButton; // creates game object to control state of load last save button

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBGM(4); // calls audio manager to play game over music

        // deactivates certain script functions to prevent unwanted action
        //GameMenu.instance.gameObject.SetActive(false);
        //BattleManager.instance.gameObject.SetActive(false);       
        PlayerController.instance.gameObject.SetActive(false);

        if (PlayerPrefs.HasKey("Current_Scene")) // checks if player prefs has key for current scene, some game data has been saved
        {
            loadLastSaveButton.SetActive(true); // sets continue button to active
        }
        else
        {
            loadLastSaveButton.SetActive(false); // sets continue button to inactive
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

    public void LoadLastSave() // creates function to handle loading last save
    {
        // destroys all loaded essential objects
        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
        // Destroy(AudioManager.instance.gameObject); // preserves audio manager
        Destroy(BattleManager.instance.gameObject);

        SceneManager.LoadScene(loadGameScene); // calls scene manager to load main menu
    }

    public void PlayButtonSound(int soundToPlay) // creates function to play UI beeps, requires int of sound to play
    {
        AudioManager.instance.PlaySFX(soundToPlay); //; plays UI beep sound from audio manager
    }
}
