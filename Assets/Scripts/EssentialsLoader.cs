// EssentialsLoader 
// handles the automatic loading of all essential elements into every scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    public GameObject UIScreen, player, gameMan, audioMan, battleMan; // creates GameObject for UI screen, player, game manager, audio manager, and battle manager

    // Start is called before the first frame update
    void Start()
    //void Awake()
    {
        Application.targetFrameRate = 60; // set a target FPS of 60

        if (PlayerController.instance == null) // checks if player object has been created
        {
            PlayerController.instance = Instantiate(player).GetComponent<PlayerController>(); // uses GetComponent to find PlayerController script
                                                                                              // instantiates static instance of PlayerController
        }

        if (GameManager.instance == null) // checks if GameManager has been created
        {
            GameManager.instance = Instantiate(gameMan).GetComponent<GameManager>(); // instantiates instance of GameManager
        }

        if (BattleManager.instance == null) // checks if BattleManager has been created
        {
            BattleManager.instance = Instantiate(battleMan).GetComponent<BattleManager>(); // instantiates instance of AudioManager
        }

        if (UIFade.instance == null) // checks if UIFade object has been created
        {
            UIFade.instance = Instantiate(UIScreen).GetComponent<UIFade>(); // uses GetComponent to find UIFade script
                                                                            // instantiates static instance of UIFade
        }

        if (AudioManager.instance == null) // checks if AudioManager has been created
        {
            AudioManager.instance = Instantiate(audioMan).GetComponent<AudioManager>(); // instantiates instance of AudioManager
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}