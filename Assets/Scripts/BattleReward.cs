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
    public int[] itemChances; // creates int array to handle chance of finding items
    private List<string> foundItems = new List<string>(); // creates string list to handle found items
    public int xpEarned; // creates int to handle EXP earned
    public int apEarned; // creates int to handle AP earned
    public int rewardGold; // creates int to handle gold earned    

    // creates variables to handle marking quests complete after battle completion
    public bool markQuestComplete;
    public string questToMark;

    public bool[] leveledUp, apLeveledUp; // creates bool array to handle if players leveled up and initializes to false by default

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

    public void OpenRewardScreen(int xp, int ap, int gold, string[] items, int[] chances) // creates function to handle display of reward screen
    {
        // assigns values in script to passed function values
        xpEarned = Mathf.RoundToInt(xp * Random.Range(0.9f, 1.1f));
        apEarned = Mathf.RoundToInt(ap * Random.Range(0.9f, 1.1f));
        rewardGold = Mathf.RoundToInt(gold * Random.Range(0.9f, 1.1f));

        // displays XP, AP, and gold earned text
        xpText.text = "Earned " + xpEarned + " EXP!";
        apText.text = "Earned " + apEarned + " AP!";
        goldText.text = "Earned " + rewardGold + " gold!";
        
        // initializes item variables and text
        itemText.text = "";
        rewardItems = items;
        itemChances = chances;
        foundItems.Clear();

        // checks if potential reward items were found
        for (int i = 0; i < rewardItems.Length; i++) // iterates through all available item rewards
        {
            if(Mathf.RoundToInt(Random.Range(1, 100)) <= itemChances[i]) // checks if roll is less than or equal to chance to receive item
            {
                Debug.Log("Found " + rewardItems[i]); // prints item found notice to debug log
                itemText.text += rewardItems[i] + "\n"; // applies current index of item text plus a line break
                foundItems.Add(rewardItems[i]); // adds item to found items list
            }
            else // checks if roll is greater than chance to receive item
            {
                Debug.Log("Didn't find " + rewardItems[i]); // prints item not found notice to debug log
            }
        }

        if (foundItems.Count == 0) // checks if no items were found
        {
            itemText.text += "None"; // notifies that no items were found
        }

        rewardScreen.SetActive(true); // shows reward screen
    }

    // NEED TO REWORK FOR NEW PARTY SYSTEM
    public void CloseRewardScreen() // creates function to handle closing of reward screen and application of rewards
    {
        for (int i = 0; i < leveledUp.Length; i++) // iterates through all elements of leveledUp array
        {
            leveledUp[i] = false; // resets each leveledUp element to false
            apLeveledUp[i] = false; // resets each apLeveledUp element to false
            GameMenu.instance.notificationText.text = ""; // resets game notification text
        }

        for(int i = 0; i < GameMenu.instance.activePartyList.Length; i++) // iterates through active party list
        {
            if (GameMenu.instance.activePartyList[i] != "Empty") // checks if player in list is not empty
            {
                CharStats activePlayer = GameMenu.instance.FindPlayerStats(GameMenu.instance.activePartyList[i]); // pulls reference to active player stats

                if (activePlayer.currentHP > 0) // checks if active party member is alive
                {
                    activePlayer.AddExp(xpEarned); // adds EXP to current player

                    if (activePlayer.leveledUp) // checks if player leveled up
                    {
                        leveledUp[i] = true; // sets leveledUp array element to true
                        //Debug.Log(activePlayer.charName + " leveled up!"); // prints level up notification to debug log

                        if (GameMenu.instance.notificationText.text.Length == 0) // checks if level up notification is empty
                        {
                            GameMenu.instance.notificationText.text += activePlayer.charName + " leveled up!"; // sets player level up notification without line break
                        }
                        else // executes if level up notification is not empty
                        {
                            GameMenu.instance.notificationText.text += "\n" + activePlayer.charName + " leveled up!"; // sets player level up notification with line break
                        }
                    }

                    activePlayer.AddAP(apEarned); // adds AP to current player

                    if (activePlayer.apLeveledUp) // checks if player ability leveled up
                    {
                        apLeveledUp[i] = true; // sets apLeveledUp array element to true
                        //Debug.Log(activePlayer.charName + " learned new abilities!"); // prints ability level up notification to debug log

                        if (GameMenu.instance.notificationText.text.Length == 0) // checks if level up notification is empty
                        {
                            GameMenu.instance.notificationText.text += activePlayer.charName + " learned new abilities!"; // sets player ability level up notification without line break
                        }
                        else // executes if level up notification is not empty
                        {
                            GameMenu.instance.notificationText.text += "\n" + activePlayer.charName + " learned new abilities!"; // sets player ability level up notification with line break
                        }
                    }
                }
            }
        }
        /*
        for (int i = 0; i < GameManager.instance.playerStats.Length; i++) // iterates through all player objects
        {
            if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) // checks if player is active
            {
                if (GameManager.instance.playerStats[i].currentHP > 0) // checks if player is alive after battle
                {
                    GameManager.instance.playerStats[i].AddExp(xpEarned); // adds EXP to current player

                    if (GameManager.instance.playerStats[i].leveledUp) // checks if player leveled up
                    {
                        leveledUp[i] = true; // sets leveledUp array element to true
                        //Debug.Log(GameManager.instance.playerStats[i].charName + " leveled up!"); // prints level up notification to debug log

                        if (GameMenu.instance.notificationText.text.Length == 0) // checks if level up notification is empty
                        {
                            GameMenu.instance.notificationText.text += GameManager.instance.playerStats[i].charName + " leveled up!"; // sets player level up notification without line break
                        }
                        else // executes if level up notification is not empty
                        {
                            GameMenu.instance.notificationText.text += "\n" + GameManager.instance.playerStats[i].charName + " leveled up!"; // sets player level up notification with line break
                        }
                    }

                    GameManager.instance.playerStats[i].AddAP(apEarned); // adds AP to current player

                    if (GameManager.instance.playerStats[i].apLeveledUp) // checks if player ability leveled up
                    {
                        apLeveledUp[i] = true; // sets apLeveledUp array element to true
                        //Debug.Log(GameManager.instance.playerStats[i].charName + " learned new abilities!"); // prints ability level up notification to debug log

                        if (GameMenu.instance.notificationText.text.Length == 0) // checks if level up notification is empty
                        {
                            GameMenu.instance.notificationText.text += GameManager.instance.playerStats[i].charName + " learned new abilities!"; // sets player ability level up notification without line break
                        }
                        else // executes if level up notification is not empty
                        {
                            GameMenu.instance.notificationText.text += "\n" + GameManager.instance.playerStats[i].charName + " learned new abilities!"; // sets player ability level up notification with line break
                        }
                    }
                }
            }
        }
        */

        if (leveledUp[0] || leveledUp[1] || leveledUp[2] || apLeveledUp[0] || apLeveledUp[1] || apLeveledUp[2]) // checks if any characters leveled up (EXP or AP)
        {
            StartCoroutine(QuestManager.instance.ShowGameNotification(2)); // calls game notification coroutine to show notice for 2 seconds
        }

        GameManager.instance.currentGold += rewardGold; // adds reward gold to inventory

        if (foundItems.Count != 0)
        {
            for (int i = 0; i < foundItems.Count; i++) // iterates through array of found items
            {
                GameManager.instance.AddItem(foundItems[i]); // adds each item in found items array to inventory
            }
        }

        rewardScreen.SetActive(false); // hides reward screen
        GameManager.instance.battleActive = false;

        if (markQuestComplete) // checks if mark quest complete flag is set
        {
            QuestManager.instance.MarkQuestComplete(questToMark); // calls mark quest complete function 
        }
    }
}
