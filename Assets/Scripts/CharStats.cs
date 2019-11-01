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
    public float[] resistances = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // creates array of floats to handle elemental resistances and initializes to default of 1
    // [0] = heat
    // [1] = freeze
    // [2] = shock
    // [3] = virus
    // [4] = chem
    // [5] = kinetic
    // [6] = water
    // [7] = quantum
    public int maxHP = 100, maxMP = 30;
    public bool inFrontRow;

    // creates various variables to handle player level and experience
    public int playerLevel = 1, playerAPLevel = 1;
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

        for (int i = 1; i < expToNextLevel.Length; i++) // iterates from index 1 to end of expToNextLevel array
        {
            expToNextLevel[i] = Mathf.FloorToInt(Mathf.Pow(i+1, 3)); // sets EXP to next level equal to level^3
                                                                     // Mathf.FloorToInt truncates anything after decimal point and preserves int
        }
        expToNextLevel[expToNextLevel.Length - 1] = 999999999; // sets last element of array (dummy element) to a huge number

        apToNextLevel = new int[] {10, 20, 40, 80, 160, 320, 640, 1280, 2560, 5120, 999999}; // initializes apToNextLevel array to default values
    }

    // Update is called once per frame
    void Update()
    {
        // *DEBUG ONLY - DISABLED* - adds EXP, AP if "K" key is pressed
        ///*
        if (Input.GetKeyDown(KeyCode.K))  
        {
            AddExp(100000); // calls AddExp function to increase EXP by 100000
            AddAP(1000); // calls AddAP function to increase AP by 1000
        }
        //*/
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

                    maxHP = Mathf.FloorToInt(Mathf.Pow((playerLevel + endurance), 2)); // increases maxHP based on endurance and level
                    
                    if(maxHP > 9999) // checks if maxHP > 9999
                    {
                        maxHP = 9999; // clamps maxHP to 9999
                    }
                    currentHP = maxHP; // restores HP to max on level up

                    maxMP = Mathf.FloorToInt(Mathf.Pow((playerLevel + tech), 2)/10); // increases maxMP based on tech and level

                    if (maxMP > 999) // checks if maxMP > 999
                    {
                        maxMP = 999; // clamps maxMP to 999
                    }
                    currentMP = maxMP; // restores MP to max on level up
                    // END WIP
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