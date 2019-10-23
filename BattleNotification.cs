// BattleNotification
// handles display of notifications in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class BattleNotification : MonoBehaviour
{
    public float awakeTime; // creates float to handle time for notification to stay awake
    private float awakeCounter; // creates counter to handle current counter value
    public Text theText; // creates Text to manage display text on notification

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (awakeCounter > 0) // checks if notification counter is running
        {
            awakeCounter -= Time.deltaTime; // decrements notification counter based on frame rate

            if (awakeCounter <= 0) // checks if notification counter <= 0, meaning counter has run out
            {
                gameObject.SetActive(false); // hides battle notification once counter runs out
            }
        } 
    }

    public void Activate() // creates function to handle activation of battle notification
    {
        gameObject.SetActive(true); // shows battle notification
        awakeCounter = awakeTime; // initializies counter to desired time
    }
}
