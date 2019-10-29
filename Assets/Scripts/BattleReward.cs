// BattleReward
// handles the display and assignment of end-of-battle rewards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class BattleReward : MonoBehaviour
{
    public static BattleReward instance; // creates static instance of BattleReward

    public Text xpText, apText, itemText, goldText; // creates Text objects to handle display of EXP and item text

    public GameObject rewardScreen; // creates GameObject to handle display of reward screen

    public string[] rewardItems; // creates string array to handle found items
    public int xpEarned; // creates int to handle EXP earned
    public int apEarned; // creates int to handle AP earned
    public int rewardGold; // creates int to handle gold earned    

    // creates variables to handle marking quests complete after battle completion
    public bool markQuestComplete;
    public string questToMark;

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets instance of BattleReward to this instance
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // *DISABLED*
        // *DEBUG ONLY* - tests opening reward screen
        if (Input.GetKeyDown(KeyCode.Y)) // checks for Y key input
        {
            OpenRewardScreen(54, new string[] { "Iron Sword", "Iron Armor" }); // calls reward screen function with certain items
        }
        */
    }

    public void OpenRewardScreen(int xp, int ap, string[] rewards, int gold) // creates function to handle display of reward screen
    {
        // assigns values in script to passed function values
        xpEarned = xp;
        apEarned = ap;
        rewardItems = rewards;
        rewardGold = gold;

        xpText.text = "Earned " + xpEarned + " EXP!"; // displays EXP earned text
        apText.text = "Earned " + apEarned + " AP!"; // displays AP earned text
        goldText.text = "Earned " + rewardGold + " gold!"; // displays gold earned text

        itemText.text = ""; // resets any previous item text

        for (int i = 0; i < rewardItems.Length; i++) // iterates through all available item rewards
        {
            itemText.text += rewards[i] + "\n"; // applies current index of item text plus a line break
        }

        rewardScreen.SetActive(true); // shows reward screen
    }

    public void CloseRewardScreen() // creates function to handle closing of reward screen and application of rewards
    {
        for(int i = 0; i < GameManager.instance.playerStats.Length; i++) // iterates through all player objects
        {
            if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) // checks if player is active
            {
                if (GameManager.instance.playerStats[i].currentHP > 0) // checks if player is alive after battle
                {
                    GameManager.instance.playerStats[i].AddExp(xpEarned); // adds EXP to current player
                    GameManager.instance.playerStats[i].AddAP(apEarned); // adds AP to current player
                }
            }
        }

        for(int i = 0; i < rewardItems.Length; i++) // iterates through array of reward items
        {
            GameManager.instance.AddItem(rewardItems[i]); // adds each item in reward items array to inventory
        }

        GameManager.instance.currentGold += rewardGold; // adds reward gold to inventory

        rewardScreen.SetActive(false); // hides reward screen
        GameManager.instance.battleActive = false;

        if (markQuestComplete) // checks if mark quest complete flag is set
        {
            QuestManager.instance.MarkQuestComplete(questToMark); // calls mark quest complete function 
        }
    }
}
