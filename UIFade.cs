// UIFade
// handles fade to/from black between scenes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class UIFade : MonoBehaviour
{
    public static UIFade instance; // creates reference to this particular instance of this UIFade class/script
                                   // "static" property only allows one version of this instance for every script in the project
                                   // allows other scripts to access this instance in order to make changes and call functions
    public Image fadeScreen; // creates Image variable to handle the UI fade screen

    // creates boolean variables to handle logic for fade screen alpha between scenes
    private bool shouldFadeToBlack;
    private bool shouldFadeFromBlack;

    public float fadeSpeed; // creates float variable to handle fade speed of fade screen

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets UIFade instance to this instance
                         // "this" keyword refers to current instance of the class
        DontDestroyOnLoad(gameObject); // prevents UIFade object from being destroyed between scenes
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFadeToBlack) // checks if shouldFadeToBlack boolean has been set
        {
            // adjusts alpha value ('a') of fade screen color in order to execute fade to black
            // uses Mathf.MoveTowards function to gradually adjust alpha value from 0f to 1f
            // multiplies Mathf.MoveTowards by fade speed to allow adjustable fade time
            // multiplies fade by Time.deltaTime function to ensure consistent fade time regardless of user framerate
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            
            if (fadeScreen.color.a == 1f) // checks if fade screen alpha has been set to 1f (fully opaque), which means fade is complete
            {
                shouldFadeToBlack = false; // resets shouldFadeToBlack boolean to false
            }
        }

        if (shouldFadeFromBlack) // checks if shouldFadeToBlack boolean has been set
        {
            // adjusts alpha value ('a') of fade screen color in order to execute fade from black
            // uses Mathf.MoveTowards function to gradually adjust alpha value from 1f to 0f
            // multiplies Mathf.MoveTowards by fade speed to allow adjustable fade time
            // multiplies fade by Time.deltaTime function to ensure consistent fade time regardless of user framerate
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));

            if (fadeScreen.color.a == 0f) // checks if fade screen alpha has been set to 0f (fully transparent), which means fade is complete
            {
                shouldFadeFromBlack = false; // resets should FadeFromBlack boolean to false
            }
        }
    }

    public void FadeToBlack() // creates FadeToBlack function to allow setting the boolean states for UI fade in other scripts
    {
        // sets/resets bool values appropriately for fade to black
        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;
    }

    public void FadeFromBlack() // creates FadeFromBlack function to allow setting the boolean states for UI fade in other scripts
    {
        // sets/resets bool values appropriately for fade from black
        shouldFadeToBlack = false;
        shouldFadeFromBlack = true;
    }
}