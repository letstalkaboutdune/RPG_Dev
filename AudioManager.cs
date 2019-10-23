// AudioManager
// handles the playback of SFX and BGM audio

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // create AudioSource arrays for sound effects and background music
    public AudioSource[] sfx;
    public AudioSource[] bgm;

    public static AudioManager instance; // creates static instance of AudioSource class/script

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets instance of AudioManager to this instance
        DontDestroyOnLoad(this.gameObject); // prevents this instance of AudioManager from being destroyed
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // *DISABLED*
        // *DEBUG ONLY* - checks for T press to test SFX playback
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            PlaySFX(4); // plays SFX at specified location
            PlayBGM(3); // plays BGM at specified location
        }
        */
    }

    public void PlaySFX(int soundToPlay) // creates function to manage playing SFX
                                         //must pass int of index of sound to play
    {
        if(soundToPlay < sfx.Length) // only calls function if passed int is within bounds of array
        {
            sfx[soundToPlay].Play(); // uses built-in Unity function to play audio
        }

    }
    public void PlayBGM(int musicToPlay) // creates function to manage playing BGM
                                         //must pass int of index of music to play
    {
        if (!bgm[musicToPlay].isPlaying) // checks if selected BGM is currently *not* playing
                                         // if statement will leave current music playing if already playing
        {
            StopMusic(); // calls stop music function to stop any music currently playing

            if (musicToPlay < bgm.Length) // only calls function if passed int is within bounds of array
            {
                bgm[musicToPlay].Play(); // uses built-in Unity function to play audio
            }
        }
    }

    public void StopMusic() // creates function to stop playing BGM
    {
        for(int i = 0; i < bgm.Length; i++) // iterates through all elements of BGM array
        {
            bgm[i].Stop(); // uses built-in Unity function to stop audio
        }
    }
}
