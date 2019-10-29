// CharStats
// handles all stats, experience, and leveling associated with a player character

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    // creates various variables to handle character stats and attributes
    public string charName, statusEffect, equippedWpn, equippedArmr, equippedAccy;
    public int currentHP, currentMP, strength, tech, endurance, agility, luck, speed;
    public int dmgWeapon, hitChance, critChance, evadeChance, blockChance, defWeapon, defTech;
    public float resHeat = 1f, resFreeze = 1f, resShock = 1f, resVirus = 1f, resChem = 1f, resKinetic = 1f, resWater = 1f, resQuantum = 1f;
    public int maxHP = 100, maxMP = 30;
    public int[] mpLvlBonus;

    // creates various variables to handle player level and experience
    public int playerLevel = 1, playerAPLevel = 1;
    public int baseEXP = 1000;
    public int currentEXP, currentAP;
    public int[] expToNextLevel, apToNextLevel;
    public int maxLevel = 100;
    public int maxAPLevel = 10;

    public string[] abilities; // creates string array to handle abilities available to character
    
    public Sprite charImage; // creates Sprite to handle character image

    public bool isMaxLevel, isMaxAPLevel; // creates bool to handle if player is at max level

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

        apToNextLevel = new int[] {0, 10, 20, 40, 80, 160, 320, 640, 1280, 2560, 999999}; // initializes apToNextLevel array to default values
    }

    // Update is called once per frame
    void Update()
    {
        // *DEBUG ONLY - DISABLED* - adds EXP, AP if "K" key is pressed
        /*
        if (Input.GetKeyDown(KeyCode.K))  
        {
            AddExp(100000); // calls AddExp function to increase EXP by 100000
            AddAP(1000); // calls AddAP function to increase AP by 1000
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
                // manages EXP and levels up player
                while (currentEXP >= expToNextLevel[playerLevel] && playerLevel < maxLevel) // continuously checks if currentEXP is larger than next level requirement and player is not max level
                {
                    currentEXP -= expToNextLevel[playerLevel]; // removes expToNextLevel from current experience
                                                               // this makes the expToNextLevel array act as EXP gain requirement rather than overall EXP requirement
                    playerLevel++; // increments player level

                    // ** NEED TO UPDATE TO SUPPORT STAT GAIN TABLES FOR ALL CHARACTERS **
                    if (playerLevel % 2 == 0) // determines whether to add to strength or defense based on odd or even levels
                                              // uses modulo function ('%') to check for even or odd
                    {
                        strength++; // increments strength on even levels
                    }
                    else
                    {
                        // defense++; // increments defense on odd levels
                    }

                    maxHP = Mathf.FloorToInt(maxHP * 1.05f); // increases maxHP based on previous value * 1.05
                    
                    if(maxHP > 9999) // checks if maxHP > 9999
                    {
                        maxHP = 9999; // clamps maxHP to 9999
                    }
                    currentHP = maxHP; // restores HP to max on level up

                    maxMP = Mathf.FloorToInt(maxMP * 1.05f); // increases maxMP based on previous value * 1.05

                    if (maxMP > 999) // checks if maxMP > 999
                    {
                        maxMP = 999; // clamps maxMP to 999
                    }
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

    public void AddAP(int apToAdd) // creates function to add AP to currentAP and check for ability level-up
    {
        if (currentHP > 0) // checks if player is alive
        {
            currentAP += apToAdd; // adds AP to our current AP

            if (playerAPLevel < maxAPLevel) // checks if playerAPLevel is less than maxAPLevel to prevent index-out-of-range errors
            {
                // manages AP and levels up player
                while (currentAP >= apToNextLevel[playerAPLevel] && playerAPLevel < maxAPLevel) // continuously checks if currentAP is larger than next AP level requirement and player is not max AP level
                {
                    currentAP -= apToNextLevel[playerAPLevel]; // removes apToNextLevel from current AP
                                                               // this makes the apToNextLevel array act as AP gain requirement rather than overall AP requirement
                    playerAPLevel++; // increments player AP level

                    // ** NEED TO UPDATE TO SUPPORT UNLOCKING ABILITIES FOR ALL CHARACTERS **
                }
            }

            if (playerAPLevel >= maxAPLevel) // checks if playerLevel is less than maxLevel to prevent unnecessary EXP gain
            {
                currentAP = 0; // sets currentEXP to 0 if player is at max level

                isMaxAPLevel = true;
            }
        }
    }
}