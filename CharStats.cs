// CharStats
// handles all stats, experience, and leveling associated with a player character

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    // creates various variables to handle character stats and attributes
    public string charName, equippedWpn, equippedArmr;
    public int currentHP, currentMP, strength, defense, wpnPwr, armrPwr;
    public int maxHP = 100, maxMP = 30;
    public int[] mpLvlBonus;

    // creates various variables to handle player level and experience
    public int playerLevel = 1;
    public int baseEXP = 1000;
    public int currentEXP;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    
    public Sprite charImage; // creates Sprite to handle character image

    public bool isMaxLevel; // creates bool to handle if player is at max level

    // Start is called before the first frame update
    void Start()
    {
        expToNextLevel = new int[maxLevel +1]; // initializes expToNextLevel array to size equal to the max number of levels +1, needed to prevent max level array out-of-range errors

        expToNextLevel[1] = baseEXP; // sets EXP requirement for level 2 based on baseEXP

        for (int i = 2; i < expToNextLevel.Length; i++) // iterates from index 2 to end of expToNextLevel array
        {
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i - 1] * 1.05f); // sets entire expToNextLevel array based on previous value * 1.05
                                                                                 // Mathf.FloorToInt truncates anything after decimal point and preserves int
        }
        expToNextLevel[expToNextLevel.Length - 1] = 999999; // sets last element of array (dummy element) to 0
    }

    // Update is called once per frame
    void Update()
    {
        
        /*
        // *DEBUG ONLY - DISABLED* - adds EXP if "K" key is pressed
        if (Input.GetKeyDown(KeyCode.K))  
        {
            AddExp(1000); // calls AddExp function to increase EXP by 1000
        }
        */
        
        
    }

    public void AddExp(int expToAdd) // creates function to add experience to currentExp and check for level-up
    {
        if (currentHP > 0) // checks if player is alive
        {
            currentEXP += expToAdd; // adds EXP to our currentEXP

            if (playerLevel < maxLevel) // checks if playerLevel is less than maxLevel to prevent index-out-of-range errors
            {

                // *disabled* replaced with while loop to enable multiple level-ups
                //if (currentEXP > expToNextLevel[playerLevel]) // checks if currentEXP is larger than next level requirement but player is less than max level
                // manages EXP and levels up player
                while (currentEXP >= expToNextLevel[playerLevel]) // continuously checks if currentEXP is larger than next level requirement
                {
                    currentEXP -= expToNextLevel[playerLevel]; // removes expToNextLevel from current experience
                                                               // this makes the expToNextLevel array act as EXP gain requirement rather than overall EXP requirement
                    playerLevel++; // increments player level

                    if (playerLevel % 2 == 0) // determines whether to add to strength or defense based on odd or even levels
                                              // uses modulo function ('%') to check for even or odd
                    {
                        strength++; // increments strength on even levels
                    }
                    else
                    {
                        defense++; // increments defense on odd levels
                    }

                    maxHP = Mathf.FloorToInt(maxHP * 1.05f); // increases maxHP based on previous value * 1.05
                    currentHP = maxHP; // restores HP to max on level up

                    maxMP += mpLvlBonus[playerLevel]; // increases maxMP based on explicitly-defined array values
                    currentMP = maxMP; // restores MP to max on level up
                }
            }

            if (playerLevel >= maxLevel) // checks if playerLevel is less than maxLevel to prevent unnecessary EXP gain
            {
                currentEXP = 0; // sets currentEXP to 0 if player is at max level

                isMaxLevel = true;
            }
        }
    }
}