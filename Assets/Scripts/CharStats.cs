// CharStats
// handles all stats, experience, and leveling associated with a player character

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    // creates various variables to handle character stats and attributes
    public string charName, statusEffect, equippedWpn, equippedOff, equippedArmr, equippedAccy;
    public int currentHP, currentSP, strength, tech, endurance, agility, luck, speed;
    public int dmgWeapon, hitChance, critChance, evadeChance, blockChance, defWeapon, defTech;
    public int critWeapon, evadeArmor, blockShield;
    public float[] resistances = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // creates array of floats to handle current elemental resistances and initializes to default of 1
    public float[] baseResistances = new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // creates array of floats to handle base elemental resistances and initializes to default of 1
    // [0] = heat
    // [1] = freeze
    // [2] = shock
    // [3] = virus
    // [4] = chem
    // [5] = kinetic
    // [6] = water
    // [7] = quantum
    public int baseHP, baseSP;
    public int maxHP = 100, maxSP = 30;
    public bool inFrontRow;
    
    // creates various variables to handle player level and experience
    public int playerLevel = 1, playerAPLevel = 1;
    public int currentEXP, currentAP;
    public int[] expToNextLevel, apToNextLevel;
    public int maxLevel = 100;
    public int maxAPLevel = 10;
    
    public float[] statGain; // creates float array to handle stat gain for each character
    public float[] floatStats; // creates float array to handle un-rounded stats for each character 

    public string[] abilities; // creates string array to handle abilities available to character
    
    //public Sprite charImage; // creates Sprite to handle character image

    public bool isMaxLevel, isMaxAPLevel; // creates bool to handle if player is at max level or max AP level
    public bool leveledUp = false; // creates bool to handle if player leveled up after XP award    
    public bool apLeveledUp = false; // creates bool to handle if player AP leveled up

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

        // initializes floatStats array to base stats
        floatStats[0] = strength;
        floatStats[1] = tech;
        floatStats[2] = endurance;
        floatStats[3] = agility;
        floatStats[4] = luck;
        floatStats[5] = speed;

        CalculateDerivedStats(); // calls function to calculate derived stats upon start
    }

    // Update is called once per frame
    void Update()
    {
        // *DEBUG ONLY - adds EXP, AP if "K" key is pressed
        /*
        if (Input.GetKeyDown(KeyCode.K))  
        {
            AddExp(10000000); // calls AddExp function to increase EXP
            AddAP(1000); // calls AddAP function to increase AP
        }
        */
    }

    public void AddExp(int expToAdd) // creates function to add experience to currentExp and check for level-up
    {
        leveledUp = false; // resets leveled up tag to false

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
                    leveledUp = true; // sets leveled up tag to true

                    IncreaseBaseStats(); // calls function to increase base stats

                    CalculateDerivedStats(); // calls function to calculate derived stats after level up

                    maxHP = baseHP + Mathf.FloorToInt((Mathf.Pow((playerLevel + endurance), 2))/3.5f); // increases maxHP based on endurance, level, and scaling factor
                    
                    if(maxHP > 9999) // checks if maxHP > 9999
                    {
                        maxHP = 9999; // clamps maxHP to 9999
                    }
                    currentHP = maxHP; // restores HP to max on level up

                    maxSP = baseSP + Mathf.FloorToInt(Mathf.Pow((playerLevel + tech), 2)/35f); // increases maxSP based on tech, level, and scaling factor

                    if (maxSP > 999) // checks if maxSP > 999
                    {
                        maxSP = 999; // clamps maxSP to 999
                    }
                    currentSP = maxSP; // restores SP to max on level up
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
        apLeveledUp = false; // resets ap leveled up tag to false

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
                    apLeveledUp = true; // sets AP leveled up tag to true

                    if (playerAPLevel >= maxAPLevel) // checks if playerLevel is less than maxLevel to prevent unnecessary EXP gain
                    {
                        currentAP = 0; // sets currentEXP to 0 if player is at max level

                        isMaxAPLevel = true; // sets is max ap level flag to true
                    }
                }
            }

            if (playerAPLevel >= maxAPLevel) // checks if playerAPLevel is less than maxAPLevel to prevent unnecessary AP gain
            {
                currentAP = 0; // sets currentAP to 0 if player is at max AP level

                isMaxAPLevel = true; // sets is max ap level flag to true
            }
        }
    }

    public void IncreaseBaseStats() // creates function to manage increase of base stats
    {
        for(int i = 0; i < statGain.Length; i++) // iterates through all elements of floating stats array
        {
            floatStats[i] += statGain[i]; // adds stat gain to float stats
        }

        // updates base stats to rounded float stats values and clamps to 99
        strength = Mathf.RoundToInt(Mathf.Clamp(floatStats[0], 0f, 99f));
        tech = Mathf.RoundToInt(Mathf.Clamp(floatStats[1], 0f, 99f));
        endurance = Mathf.RoundToInt(Mathf.Clamp(floatStats[2], 0f, 99f));
        agility = Mathf.RoundToInt(Mathf.Clamp(floatStats[3], 0f, 99f));
        luck = Mathf.RoundToInt(Mathf.Clamp(floatStats[4], 0f, 99f));
        speed = Mathf.RoundToInt(Mathf.Clamp(floatStats[5], 0f, 99f));
    }

    public void CalculateDerivedStats() // creates function to calculate player derived stats
    {
        critChance = (agility + luck + critWeapon) / 3; // calculates player crit chance
        
        evadeChance = (agility + speed + evadeArmor) / 3; // calculates player evade chance

        if(equippedOff != "" && GameManager.instance.GetItemDetails(equippedOff).isShield) // checks if player offhand is not blank and a shield is equipped
        {
            blockChance = (strength + blockShield) / 2; // calculates player block chance
        }
        else 
        {
            blockChance = 0; // sets block chance to 0
        }

        if (BattleManager.instance.battleActive) // checks if battle is active
        {
            Debug.Log("charName = " + charName); // prints character name to debug log

            for (int i = 0; i < BattleManager.instance.activeBattlers.Count; i++) // increments through all active battlers
            {
                Debug.Log("activeBattler name = " + BattleManager.instance.activeBattlers[i].name); // prints active battler name
                
                if ((charName + "(Clone)") == BattleManager.instance.activeBattlers[i].name) // executes if current battler matches this character name
                {
                    // applies derived stats to active battler
                    BattleManager.instance.activeBattlers[i].critChance = critChance;
                    BattleManager.instance.activeBattlers[i].evadeChance = evadeChance;
                    BattleManager.instance.activeBattlers[i].blockChance = blockChance;
                }
            }
        }
    }
}