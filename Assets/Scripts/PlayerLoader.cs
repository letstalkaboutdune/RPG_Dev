// PlayerLoader
// *replaced by EssentialsLoader*
// handles loading a player into a scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    public GameObject player; // creates a reference to the game object attached to this script, in this case the player

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerController.instance == null) // checks if the static instance of PlayerController is equal to null
                                              // if null, means no player has been loaded into the scene
        {
            Instantiate(player); // instantiates player object into the world
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}