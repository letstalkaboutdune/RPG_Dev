// BattleManager
// handles the wide-reaching execution of battle scenes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library
using UnityEngine.SceneManagement; // includes Unity SceneManagement library
using SpriteGlow; // includes SpriteGlow function

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance; // creates static instance of BattleManager

    public bool battleActive; // creates bool to handle if battle is active
    
    public GameObject battleScene; // creates game object to manage state of battleScene and camera position

    public Transform[] playerPositions, enemyPositions; // creates Transform array to manage player, enemy positions in battle
    
    public BattleChar[] playerPrefabs, enemyPrefabs; // creates BattleChar array to manage player, enemy characters

    public List<BattleChar> activeBattlers = new List<BattleChar>(); // creates new list to handle list of active battle characters

    public int currentTurn; // creates int to handle current turn
    public bool turnWaiting; // creates bool to handle if we're waiting on a turn to end

    public GameObject uiButtonsHolder, statsHolder, actionButtonsHolder; // creates game objects to handle display of battle UI buttons and stats

    public BattleMove[] movesList; // creates BattleMove object to handle complete moves list

    public GameObject enemyAttackEffect; // creates game object to handle attack effect particles
    
    public DamageNumber theDamageNumber; // creates DamageNumber object to handle manipulating damage number

    public Text[] playerName, playerHP, playerMP; // creates Text arrays to handle player stats in battle UI

    // creates game objects and BattleTargetButton objects to manage player target menu
    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    // creates game object and BattleMagicSelect button objects to manage player magic menu
    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;

    public BattleNotification battleNotice; // creates reference to battle notification object
    public bool battleNoticeActive; // creates bool to handle if battle notice is active

    public int chanceToFlee = 35; // creates int to handle chance to fleeing from battle
    private bool fleeing; // creates bool to handle whether party is fleeing

    // creates various objects and variables to handle item menu
    public GameObject itemMenu;
    public ItemButton[] battleItemButtons;
    public string battleSelectedItem;
    public Item battleActiveItem, battleLastItem;
    
    public Text battleItemName, battleItemDescription, battleUseButtonText;
    public GameObject battleItemCharChoiceMenu;
    public Text[] battleItemCharChoiceNames;

    public string gameOverScene; // creates string to handle transition to game over screen

    // creates variables to handle providing battle rewards
    public int rewardXP, rewardAP, rewardGold;
    public string[] rewardItems;

    public bool cannotFlee; // creates bool to handle whether party can flee from battle

    public AttackEffect effectCheck; // creates AttackEffect to check name of spell effects
    public float waitCounter; // creates float to handle wait counter for attack animations

    public bool playerActing = false; // creates bool to handle if player is taking a turn

    // creates variables to manage sprite fades
    public int selectedSprite;
    public bool spriteFadeOut;

    // WIP
    // creates variables to manage results of attack rolls
    public bool attackHit = false, attackCrit = false, attackEvaded = false, attackBlocked = false, statusResisted = false;
    public float critMulti = 1f, resMulti = 1f;
    public int moveIndex, movePower, selectedTarget, damageRoll;
    // END WIP
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets instance of BattleManager to this instance
        DontDestroyOnLoad(gameObject); // prevents BattleManager object from being destroyed on scene load
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // *DEBUG ONLY - DISABLED* - checks for T input to test battle start
        if (Input.GetKeyDown(KeyCode.T))  
        {
            BattleStart(new string[] {"Spider", "Spider", "Spider"}, false); // calls battle start function with test enemies
        }
        // *END DEBUG
        */

        if (battleActive) // checks if battle is active
        {
            if (turnWaiting) // checks if we are waiting on a turn
            {
                GreenSpriteGlow(currentTurn); // shows green sprite glow on current turn active battler

                if (activeBattlers[currentTurn].isPlayer) // checks if active battler is a player
                {
                    if (!playerActing) // checks if player is not in the middle of a move
                    {
                        uiButtonsHolder.SetActive(true); // enables UI buttons since a player has yet to move
                    }
                }
                else
                {
                    uiButtonsHolder.SetActive(false); // hides UI buttons since a player is inactive

                    StartCoroutine(EnemyMoveCo()); // calls EnemyMoveCo coroutine to handle enemy turn
                }
            }                            
             
            // *DEBUG ONLY* - checks for N input to test turn order
            if (Input.GetKeyDown(KeyCode.N))
            {
               NextTurn(); // calls next turn function
            } 
            // END DEBUG    
            
            // WIP
            /*
            if (spriteFadeOut) // checks if spriteFadeOut is true, meaning a sprite needs to fade out
            {
                //fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
                //activeBattlers[battlerToHide].theSprite.GetComponent<SpriteGlowEffect>().enabled = false; // hides sprite glow
                
                Color fadeOutColor = activeBattlers[selectedSprite].theSprite.GetComponent<SpriteGlowEffect>().GlowColor; // copies current sprite glow color to new color variable

                activeBattlers[selectedSprite].theSprite.GetComponent<SpriteGlowEffect>().GlowColor = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, Mathf.MoveTowards(fadeOutColor.a, 0f, 5f * Time.deltaTime)); // fades sprite out over <> seconds

                if (activeBattlers[selectedSprite].theSprite.GetComponent<SpriteGlowEffect>().GlowColor.a == 0f) // checks if sprite glow alpha is 0f (fully transparent), meaning sprite fade is complete
                {
                    spriteFadeOut = false; // resets spriteFade boolean to false
                }
            }
            */
            // END WIP
        }
    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee, int musicToPlay) // creates function to manage battle start
                                                                         // requires string array of enemy names to execute
                                                                         // also requires bool for whether party can flee
    {
        if (!battleActive) // checks if a battle is already underway
        {
            cannotFlee = setCannotFlee; // sets cannot flee flag to passed setCannotFlee bool
            
            battleActive = true; // sets battleActive to true to allow battle to start

            GameManager.instance.battleActive = true; // sets battleActive in game manager to true to prevent player movement

            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z); // sets position of battle manager to current camera position
                                                                                                                                        // uses built-in Unity function of Camera.main to pull transform.position
             // sets battleScene and menu objects active to show battle UI
            battleScene.SetActive(true);
            statsHolder.SetActive(true);

            AudioManager.instance.PlayBGM(musicToPlay); // starts battle music based on passed int

            for (int i = 0; i < playerPositions.Length; i++) // iterates through all elements of player positions array
            {
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) // checks if player at that position is active in hierarchy by checking player stats in game manager
                {
                    for(int j = 0; j < playerPrefabs.Length; j++) // iterates through all elements of player prefabs array
                    {
                        if(playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName) // checks if active player name matches any player prefab name
                        {
                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation); // instantiates new player at set position
                            newPlayer.transform.parent = playerPositions[i]; // sets new player instance as a child of player positions array
                            activeBattlers.Add(newPlayer); // adds new player element to active battlers list using built-in Add function

                            CharStats thePlayer = GameManager.instance.playerStats[i]; // stores information from player stats index for easy assignment

                            // WIP - added new stats
                            // assigns all player stats for the player
                            activeBattlers[i].currentHP = thePlayer.currentHP;
                            activeBattlers[i].maxHP = thePlayer.maxHP;
                            activeBattlers[i].currentMP = thePlayer.currentMP;
                            activeBattlers[i].maxMP = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strength;
                            activeBattlers[i].tech = thePlayer.tech;
                            activeBattlers[i].endurance = thePlayer.endurance;
                            activeBattlers[i].agility = thePlayer.agility;
                            activeBattlers[i].luck = thePlayer.luck;
                            activeBattlers[i].speed = thePlayer.speed;
                            activeBattlers[i].dmgWeapon = thePlayer.dmgWeapon;
                            activeBattlers[i].hitChance = thePlayer.hitChance;
                            activeBattlers[i].critChance = thePlayer.critChance;
                            activeBattlers[i].evadeChance = thePlayer.evadeChance;
                            activeBattlers[i].blockChance = thePlayer.blockChance;
                            activeBattlers[i].defWeapon = thePlayer.defWeapon;
                            activeBattlers[i].defTech = thePlayer.defTech;
                            activeBattlers[i].resHeat = thePlayer.resHeat;
                            activeBattlers[i].resFreeze = thePlayer.resFreeze;
                            activeBattlers[i].resShock = thePlayer.resShock;
                            activeBattlers[i].resVirus = thePlayer.resVirus;
                            activeBattlers[i].resChem = thePlayer.resChem;
                            activeBattlers[i].resKinetic = thePlayer.resKinetic;
                            activeBattlers[i].resWater = thePlayer.resWater;
                            activeBattlers[i].resQuantum = thePlayer.resQuantum;
                            //activeBattlers[i].defense = thePlayer.defense;
                            //activeBattlers[i].wpnPower = thePlayer.wpnPwr;
                            //activeBattlers[i].armrPower = thePlayer.armrPwr;
                            // END WIP

                            // WIP
                            // ** NEED TO ADD FOR LOOP TO ADD ALL MOVES AVAILABLE BASED ON ABILITIES LIST AND ABILITY LEVEL **
                            activeBattlers[i].movesAvailable = new string[thePlayer.playerAPLevel]; // initializes active battler moves list array to size equal to player AP level

                            for (int k = 0; k < thePlayer.playerAPLevel; k++) // iterates as many times as the player ability level
                            {
                                activeBattlers[i].movesAvailable[k] = thePlayer.abilities[k]; // builds the player's move list based on the abilities unlocked for that ability level
                            }
                            // END WIP
                        }
                    }
                }
            }

            for (int i = 0; i < enemiesToSpawn.Length; i++) // iterates through all elements of enemies to spawn array
            {
                if(enemiesToSpawn[i] != "") // checks if enemy in array is not blank
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++) // iterates through all elements of enemy prefabs array
                    {
                        if(enemyPrefabs[j].charName == enemiesToSpawn[i]) // checks if any enemy in prefabs matches current element of enemies to spawn array
                        {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation); // instantiates new enemy at set position
                            newEnemy.transform.parent = enemyPositions[i]; // sets new enemy instance as a child of enemy positions array
                            activeBattlers.Add(newEnemy); // adds new player element to active battlers list
                        }
                    }
                }
            }

            AddSpriteGlow(); // calls function to add sprite glow to all active battlers

            // initializes basic variables controlling combat turns
            turnWaiting = true;
            currentTurn = Random.Range(0, activeBattlers.Count); // randomizes who gets to go first using built-in Random.Range function
                                                                 // since activeBattlers is of type List, must use Count (for list) rather than Length (for array)
                                                                 //currentTurn = 0; // sets turn to 0
            UpdateBattle(); // calls function to update battle to reflect player dead upon battle entry

            UpdateUIStats(); // calls function to update UI battle stats
        }
    }

    public void NextTurn() // creates function to handle next turn in combat
    {
        Debug.Log("**** NEXT TURN ****"); // prints next turn notification to debug log

        HideSpriteGlow(currentTurn); // hides sprite glow on active battler ending turn
        
        currentTurn++; // increments current turn
        
        if(currentTurn >= activeBattlers.Count) // checks if current turn is greater than active battlers count
        {
            currentTurn = 0; // resets current turn to 0
        }

        turnWaiting = true; // sets turn waiting to true

        UpdateBattle(); // updates information in battle whenever a turn completes
        UpdateUIStats(); // calls function to update UI battle stats
    }

    public void UpdateGameManager() // creates function to handle updating game manager based on battle stats
    {
        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active players
        {
            if (activeBattlers[i].isPlayer) // checks if active battler is player
            {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++) // iterates through all elements of playerStats array in GameManager
                {
                    if (activeBattlers[i].charName == GameManager.instance.playerStats[j].charName) // checks if active battler name matches char name in game manager
                    {
                        // WIP - added new stats
                        // updates current player stats index in game manager with active battler
                        GameManager.instance.playerStats[i].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[i].maxHP = activeBattlers[i].maxHP;
                        GameManager.instance.playerStats[i].currentMP = activeBattlers[i].currentMP;
                        GameManager.instance.playerStats[i].maxMP = activeBattlers[i].maxMP;
                        GameManager.instance.playerStats[i].strength = activeBattlers[i].strength;
                        GameManager.instance.playerStats[i].tech = activeBattlers[i].tech;
                        GameManager.instance.playerStats[i].endurance = activeBattlers[i].endurance;
                        GameManager.instance.playerStats[i].agility = activeBattlers[i].agility;
                        GameManager.instance.playerStats[i].luck = activeBattlers[i].luck;
                        GameManager.instance.playerStats[i].speed = activeBattlers[i].speed;
                        GameManager.instance.playerStats[i].dmgWeapon = activeBattlers[i].dmgWeapon;
                        GameManager.instance.playerStats[i].hitChance = activeBattlers[i].hitChance;
                        GameManager.instance.playerStats[i].critChance = activeBattlers[i].critChance;
                        GameManager.instance.playerStats[i].evadeChance = activeBattlers[i].evadeChance;
                        GameManager.instance.playerStats[i].blockChance = activeBattlers[i].blockChance;
                        GameManager.instance.playerStats[i].defWeapon = activeBattlers[i].defWeapon;
                        GameManager.instance.playerStats[i].defTech = activeBattlers[i].defTech;
                        GameManager.instance.playerStats[i].resHeat = activeBattlers[i].resHeat;
                        GameManager.instance.playerStats[i].resFreeze = activeBattlers[i].resFreeze;
                        GameManager.instance.playerStats[i].resShock = activeBattlers[i].resShock;
                        GameManager.instance.playerStats[i].resVirus = activeBattlers[i].resVirus;
                        GameManager.instance.playerStats[i].resChem = activeBattlers[i].resChem;
                        GameManager.instance.playerStats[i].resKinetic = activeBattlers[i].resKinetic;
                        GameManager.instance.playerStats[i].resWater = activeBattlers[i].resWater;
                        GameManager.instance.playerStats[i].resQuantum = activeBattlers[i].resQuantum;
                        //GameManager.instance.playerStats[i].defense = activeBattlers[i].defense;
                        //GameManager.instance.playerStats[i].wpnPwr = activeBattlers[i].wpnPower;
                        //GameManager.instance.playerStats[i].armrPwr = activeBattlers[i].armrPower;
                        // END WIP
                    }
                }
            }
        }
    }

    public void UpdateBattle() // creates function to handle updating information in battle, including from game manager to battle stats
    {
        // creates bools to handle checks if players or enemies are dead, sets default of true
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers
        {
            if(activeBattlers[i].currentHP < 0) // checks if current HP is < 0
            {
                activeBattlers[i].currentHP = 0; // sets current HP to 0
            }

            if(activeBattlers[i].currentHP == 0) // checks if current HP = 0
            {
                if (activeBattlers[i].isPlayer) // checks if dead battler is a player
                {
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].deadSprite; // sets active player sprite to "dead"
                }
                else // executes if dead battler is an enemy
                {
                    activeBattlers[i].EnemyFade(); // runs enemy fade function
                }
            }
            else // executes if battler current HP > 0, so is not dead
            {
                if (activeBattlers[i].isPlayer) // checks if current battler is player
                {
                    allPlayersDead = false; // sets allPlayersDead tag to false, since at least one player is not dead
                    
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].aliveSprite; // sets active player sprite to "alive"
                }
                else
                {
                    allEnemiesDead = false; // sets allEnemiesDead tag to false, since at least one enemy is not dead
                }
            }
        }

        if(allEnemiesDead || allPlayersDead) // checks if all enemies or players are dead, so battle is over
        {
            if (allEnemiesDead)
            {
                StartCoroutine(EndBattleCo()); // calls coroutine to end battle in success
            }
            else
            {
                StartCoroutine(GameOverCo()); // calls coroutine to end battle in failure
            }
        }
        else // executes if battle is not over
        {
            while (activeBattlers[currentTurn].currentHP == 0) // executes if current battler HP = 0
                                                               // set as while loop to handle multiple dead battlers
            {
                currentTurn++; // increments current turn to skip dead battler
                
                if(currentTurn >= activeBattlers.Count) // checks if end of turn order has been reached by comparing to # of battlers
                {
                    currentTurn = 0; // resets turn order to 0
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo() // creates function to handle enemy move delay
                                     // IEnumerator is a co-routine, which executes out of order
    {
        turnWaiting = false; // sets turn waiting to false since enemy is doing a move

        yield return new WaitForSeconds(1f); // forces a one-second wait

        List<int> players = new List<int>(); // creates a list of type int to handle list of active players

        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers
        {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0) // checks if current battler is player and HP > 0
            {
                players.Add(i); // adds current index to players list
            }
        }

        selectedTarget = players[Random.Range(0, players.Count)]; // selects a player as a target, use RNG to pick from the active player list

        RedSpriteGlow(selectedTarget); // shows red sprite glow on current selected target

        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length); // picks a random move from this battler's move list
        //movePower = 0; // creates int to store power of a move, initializes to 0 by default

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation); // instantiates attacker white particle effect
        
        for (int i = 0; i < movesList.Length; i++) // iterates through all moves in moves list
        {
            if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack]) // checks if selected move is contained in move list
            {
                moveIndex = i; // assigns current value of i to moveIndex for use in damage rolls

                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation); // instantiates attack effect on selected target

                yield return new WaitForSeconds(movesList[i].theEffect.effectLength); // wait for length of time so that attack animation can finish

                AttackRolls(); // calls function to manage various attack rolls
            }
        }

        //DealDamage(selectedTarget, damageRoll); // calls function to deal damage to player based on selected target and previous damage roll

        yield return new WaitForSeconds(1f); // forces a one-second wait

        HideSpriteGlow(selectedTarget); // hides sprite glow on current selected target
        
        NextTurn(); // calls next turn function
    }    

    public IEnumerator PlayerMoveCo(string moveName, int playerTarget) // creates function to handle players attacking
    {
        selectedTarget = playerTarget;
        playerActing = true; // sets playerActing true to stop update loop from acting on UI
        //moveIndex = 0; // create local int variable to store found move index in moveList
        
        uiButtonsHolder.SetActive(false); // hides action buttons once player action starts
        targetMenu.transform.localScale = new Vector3(0, 0, 0); // adjusts scale of targetMenu to 0 to prevent multiple clicks

        //movePower = 0; // creates int to store power of a move, initializes to 0 by default

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        //Debug.Log("Entering for loop."); // print various debug logs

        for (int i = 0; i < movesList.Length; i++) // iterates through all moves in moves list
        {            
            if (movesList[i].moveName == moveName) // checks if selected move is contained in move list
            {
                moveIndex = i; // assign i value to moveIndex when move is found
                
                RedSpriteGlow(selectedTarget); // shows red sprite glow on current selected target
                
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation); // instantiates attack effect on selected target

                movePower = movesList[i].movePower; // pulls move power from moves list and stores to movePower variable                               
            }
        }

        //Debug.Log("Starting wait = " + movesList[moveIndex].theEffect.effectLength); // print wait time to debug log        
        yield return new WaitForSeconds(movesList[moveIndex].theEffect.effectLength); // wait for length of time so that attack animation can finish

        AttackRolls(); // calls function to manage various attack rolls

        //Debug.Log("Dealing damage."); // print status to debug log        
        //DealDamage(); // calls function to deal damage to enemy

        yield return new WaitForSeconds(1f); // forces short wait to prevent visual glitches

        //Debug.Log("Finishing player turn."); // print status to debug log        

        playerActing = false; // resets playerActing false to allow update loop to act on UI

        targetMenu.SetActive(false); // hides target button UI to prevent multi-clicks, will get re-enabled on next update if player turn
        targetMenu.transform.localScale = new Vector3(1, 1, 1); // adjusts scale of targetMenu to 0

        HideSpriteGlow(selectedTarget); // hides sprite glow on current selected target
        NextTurn(); // calls next turn function
    }

    public void DealDamage() // creates function to handle applying damage to HP
    {
        // prints damage calculation in debug log
        Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageRoll + " damage to " + activeBattlers[selectedTarget].charName);

        // subtracts target's current HP by calculated damage
        activeBattlers[selectedTarget].currentHP -= damageRoll;

        // instantiates damage number in the battle scene on top of the target
        Instantiate(theDamageNumber, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation).SetDamage(damageRoll);

        UpdateUIStats(); // calls function to update UI battle stats
    }

    public void UpdateUIStats() // creates function to update displayed stats in battle UI
    {
        for(int i = 0; i < playerName.Length; i++) // iterates through all elements of playerName array
        {
            if(activeBattlers.Count > i) // checks if length of active battlers list is greater than i to prevent out-of-bounds errors
            {
                if (activeBattlers[i].isPlayer) // checks if active battler is a player
                {
                    BattleChar playerData = activeBattlers[i]; // create a BattleChar reference to current active battler data

                    playerName[i].gameObject.SetActive(true); // shows player name object if a player

                    // updates player name, HP, and MP on UI, clamps HP and MP to never be < 0
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
                }
                else 
                {
                    playerName[i].gameObject.SetActive(false); // hides player name object if not a player
                }
            } else
            {
                playerName[i].gameObject.SetActive(false); // hides player name object if not a player
            }            
        }
    }

    public void OpenTargetMenu(string moveName) // creates function to open player target menu in battle UI
    {
        targetMenu.SetActive(true); // shows battle target menu

        List<int> enemies = new List<int>(); // creates new int list to store list of enemy indices

        for(int i = 0; i < activeBattlers.Count; i++) // iterates through list of all active battlers
        {
            if (!activeBattlers[i].isPlayer) // detects enemy in list by checking if isPlayer tag is false
            {
                enemies.Add(i); // adds current index value to enemies list
            }
        }

        for(int i = 0; i < targetButtons.Length; i++) // iterates through array of all target buttons
        {
            if (enemies.Count > i && activeBattlers[enemies[i]].currentHP > 0) // checks if total enemy count is greater than i and enemy current HP is > 0
            {
                targetButtons[i].gameObject.SetActive(true); // shows target button at index i
                targetButtons[i].moveName = moveName; // sets move name of target button
                targetButtons[i].activeBattlerTarget = enemies[i]; // sets enemy index of target button
                targetButtons[i].targetName.text = activeBattlers[enemies[i]].charName; // updates enemy name in target button text
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false); // hides target button at index i
            }
        }
    }

    public void CloseTargetMenu() // creates function to close target menu
    {
        targetMenu.SetActive(false); // hides target menu
    }

    public void OpenMagicMenu() // creates function to open player magic menu in battle UI
    {
        magicMenu.SetActive(true); // shows player magic menu

        for(int i = 0; i < magicButtons.Length; i++) // iterates through all elements in magicButtons array
        {
            if (activeBattlers[currentTurn].movesAvailable.Length > i) // checks if player has magic spells available
            {
                magicButtons[i].gameObject.SetActive(true); // shows magic button at index i
                magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i]; // sets spell name of spell button
                magicButtons[i].nameText.text = magicButtons[i].spellName; // updates spell name in spell button text
                
                for(int j = 0; j < movesList.Length; j++) // iterates through all elements in move list
                {
                    if(movesList[j].moveName == magicButtons[i].spellName) // checks if current element in move list is equal to current spell name on spell button
                    {
                        magicButtons[i].spellCost = movesList[j].moveCost; // sets spell button cost to cost of current element in move list 
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString(); // updates spell button text to spell cost
                    }
                }
            }
            else
            {
                magicButtons[i].gameObject.SetActive(false); // hides magic button at index i
            }
        }
    }

    public void CloseMagicMenu() // creates function to close magic menu
    {
        magicMenu.SetActive(false); // hides magic menu
    }

    public void Flee() // creates function to handle fleeing from battle
    {
        if (cannotFlee) // checks if party cannot flee from battle
        {
            battleNotice.theText.text = "Can't flee this battle!"; // sets battle notification text to escape failure
            battleNotice.Activate(); // activates battle notification text in menu
        }
        else // executes if party can flee from battle
        {
            int fleeSuccess = Random.Range(0, 100); // uses RNG from 0 to 100 to determine flee success

            if (fleeSuccess < chanceToFlee) // checks if flee success result is lower than threshold set by chance to flee
                                            // flee is successful when fleeSuccess < chanceToFlee
            {
                fleeing = true; // sets fleeing booleran to true

                HideSpriteGlow(currentTurn); // hides sprite glow from current turn active battler

                StartCoroutine(EndBattleCo()); // calls end battle coroutine
            }
            else
            {
                NextTurn(); // moves to next character's turn
                battleNotice.theText.text = "Couldn't escape!"; // sets battle notification text to escape failure
                battleNotice.Activate(); // activates battle notification text in menu
            }
        } 
    }

    public void OpenItemsMenu() // creates function to handle opening item menu in battle
    {
        itemMenu.SetActive(true); // shows items menu
        ShowBattleItems(); // calls show battle items function
    }

    public void CloseItemsMenu() // creates function to handle closing item menu in battle
    {
        itemMenu.SetActive(false); // hides items menu
        CloseBattleItemCharChoice(); // calls function to hide char choice menu
        CloseActionButtons(); // closes item action panel

        battleActiveItem = null; // resets active item to null

        // resets item details in menu to default values
        battleItemName.text = "Please select an item.";
        battleItemDescription.text = "";
    }

    public void ShowBattleItems() // creates function to handle sorting and showing battle item buttons
    {
        GameManager.instance.SortItems(); // sorts items before displaying

        for (int i = 0; i < battleItemButtons.Length; i++) // iterates through all elements in itemButtons
        {
            battleItemButtons[i].buttonValue = i; // sets item button value to current iteration

            if (GameManager.instance.itemsHeld[i] != "") // checks if element in itemsHeld array is empty
            {
                battleItemButtons[i].buttonImage.gameObject.SetActive(true); // sets button image to active

                battleItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite; // calls GetItemDetails function in GameManager to determine that item element
                                                                                                                                             // pulls sprite from itemsHeld array in GameManager and assigns to button image
                battleItemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString(); // pulls number from numberOfItems array in GameManager and assigns to button amount
            }
            else // executes if element in itemsHeld array is blank
            {
                battleItemButtons[i].buttonImage.gameObject.SetActive(false); // sets button image to inactive

                battleItemButtons[i].amountText.text = ""; // sets amount text to blank
            }
        }
   }

    public void SelectBattleItem(Item newItem/*, int newItemQuantity*/) // creates function to update item name, item description, and use button name in inventory when item is selected
                                                                    // must pass Item object and int to function
    {
        if (newItem != null) // checks for null item selection to prevent out-of-range errors
        {
            if (newItem != battleLastItem) // checks if new selected item is not same as last selected item
            {
                CloseBattleItemCharChoice(); // closes item character choice panel
            }

            battleLastItem = newItem; // updates last battle item check
            battleActiveItem = newItem; // sets passed Item object to activeItem
            // battleActiveItemQuantity = newItemQuantity;

            OpenActionButtons(); // shows item action panel

            if (battleActiveItem.isItem) // checks if activeItem is of type item
            {
                battleUseButtonText.text = "Use"; // sets use button text to "Use"
            }

            if (battleActiveItem.isWeapon || battleActiveItem.isArmor) // checks if activeItem is of type weapon or armor
            {
                battleUseButtonText.text = "Equip"; // sets use button text to "Equip"
            }

            // updates selected item name and description
            battleItemName.text = battleActiveItem.itemName;
            battleItemDescription.text = battleActiveItem.description;
        }
        else // executes if selected item is null
        {

            battleActiveItem = null; // resets battle active item
            CloseActionButtons(); // hides item action panel
            
            // resets item description to defaults
            battleItemName.text = "Please select an item.";
            battleItemDescription.text = "";
        }
    }

    public void OpenBattleItemCharChoice() // creates function to open item character choice menu and update names
    {
        if (battleActiveItem != null)
        {
            battleItemCharChoiceMenu.SetActive(true); // activates item character choice menu

            for (int i = 0; i < battleItemCharChoiceNames.Length; i++) // iterates through all character names
            {
                battleItemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName; // replaces text of character buttons with player names from player stats objects array

                battleItemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy); // sets character button active/inactive based on whether character is active in hierarchy
            }
        }
    }

    public void CloseBattleItemCharChoice() // creates function to close item character choice menu
    {
        battleItemCharChoiceMenu.SetActive(false); // de-activates item character choice menu
    }

    public void UseBattleItem(int selectChar) // creates function to handle use of item character choice menu buttons
    {
        battleNoticeActive = false; // resets battle notice active to false
        
        //Debug.Log("Batt.Use passed item = " + battleActiveItem.itemName); // prints item name to debug log

        battleActiveItem.Use(selectChar); // calls Use function to handle use of item on selected char

        if (battleNoticeActive)
        {
            battleNotice.Activate();
            battleNoticeActive = false;
        }
        else
        {
            Debug.Log(battleActiveItem.itemName + " was used."); // prints debug text to notify on item use

            /*
            // WIP
            battleActiveItemQuantity--; // decrements active item quantity
            if (battleActiveItemQuantity <= 0) // checks if active 
            {
                battleActiveItem = null; // resets active item to null
                CloseActionButtons(); // closes item action panel

                // resets item details in menu to default values
                battleItemName.text = "Please select an item.";
                battleItemDescription.text = "";
            }
            */

            CloseItemsMenu(); // calls function to close items menu
            CloseActionButtons(); // closes item action panel

            battleActiveItem = null; // resets active item to null

            // resets item details in menu to default values
            battleItemName.text = "Please select an item.";
            battleItemDescription.text = "";

            UpdateBattle(); // calls extra battle stats update, since item acts on game manager
            NextTurn(); // moves to next turn in battle
        }


    }

    public IEnumerator EndBattleCo() // creates IEnumerator coroutine to end battle in victory
    {
        battleActive = false; // sets battle active to false to prevent more turns
        turnWaiting = false; // sets turn waiting to false to prevent issues loading battle

        // hides all battle UI
        uiButtonsHolder.SetActive(false);
        statsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        itemMenu.SetActive(false);

        yield return new WaitForSeconds(0.5f); // forces half-second wait for animations, etc. to end

        UIFade.instance.FadeToBlack(); // calls UI fade to black function

        yield return new WaitForSeconds(1.5f); // forces 1.5-second wait for UI fade to occur

        battleScene.SetActive(false); // hides battle scene

        UpdateGameManager(); // update game manager with latest active battler data

        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers
        {
            Destroy(activeBattlers[i].gameObject); // destroys each active battler object
        }

        UIFade.instance.FadeFromBlack(); // calls UI fade from black function
     
        battleScene.SetActive(false); // hides battle scene

        activeBattlers.Clear(); // clears active battlers list
        currentTurn = 0; // resets current turn

        if (fleeing) // checks if party is fleeing
        {
            GameManager.instance.battleActive = false; // notifies game manager that battle is no longer active
            fleeing = false; // clears fleeing tag after battle
        }
        else // calls reward screen if party is not fleeing
        {
            BattleReward.instance.OpenRewardScreen(rewardXP, rewardAP, rewardItems, rewardGold); // calls battle reward function based on passed XP, AP, items, and gold
        }

        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay); // changes BGM back to current scene
    }

    public IEnumerator GameOverCo() // creates IEnumerator coroutine to end battle in game over
    {
        battleActive = false; // sets battle active to false to prevent more turns
        turnWaiting = false; // sets turn waiting to false to prevent issues loading battle

        // hides all battle UI
        uiButtonsHolder.SetActive(false);
        statsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        itemMenu.SetActive(false);

        UIFade.instance.FadeToBlack(); // calls UI fade to black function

        yield return new WaitForSeconds(1.5f); // forces 1.5-second wait for UI fade to occur

        battleScene.SetActive(false); // hides battle scene

        SceneManager.LoadScene(gameOverScene); // loads game over scene
    }

    public void OpenActionButtons() // creates function to handle opening of battle item menu action panel
    {
        actionButtonsHolder.SetActive(true); // shows item action panel
    }

    public void CloseActionButtons() // creates function to handle closing of battle item menu action panel
    {
        actionButtonsHolder.SetActive(false); // hides item action panel
    }
    
    public void PlayButtonSound(int soundToPlay) // creates function to play UI beeps, requires int of sound to play
    {
        AudioManager.instance.PlaySFX(soundToPlay); //; plays UI beep sound from audio manager
    }

    public void AddSpriteGlow() // creates function to add sprite glow script to all active battlers
    {
        for(int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers in list
        {
            activeBattlers[i].theSprite.gameObject.AddComponent<SpriteGlowEffect>(); // adds sprite glow component
            activeBattlers[i].theSprite.GetComponent<SpriteGlowEffect>().enabled = false; // hides sprite glow component           
            activeBattlers[i].theSprite.GetComponent<SpriteGlowEffect>().DrawOutside = true; // sets sprite glow to render outside the sprite
        }
    }

    public void GreenSpriteGlow(int battlerToGreen) // creates function to show green sprite glow on active battler
    {
        activeBattlers[battlerToGreen].theSprite.GetComponent<SpriteGlowEffect>().enabled = true; // shows sprite glow
        activeBattlers[battlerToGreen].theSprite.GetComponent<SpriteGlowEffect>().GlowColor = Color.green; // sets sprite glow color to green
    }

    public void RedSpriteGlow(int battlerToRed) // creates function to show red sprite glow on selected target
    {
        activeBattlers[battlerToRed].theSprite.GetComponent<SpriteGlowEffect>().enabled = true; // shows sprite glow
        activeBattlers[battlerToRed].theSprite.GetComponent<SpriteGlowEffect>().GlowColor = Color.red; // sets sprite glow color to red
    }

    public void HideSpriteGlow(int battlerToHide) // creates function to hide sprite glow on battler
    {
        // WIP
        /*
        selectedSprite = battlerToHide; // passes battler to hide to selected sprite
        spriteFadeOut = true; // sets sprite fade tag to true
        */
        // END WIP

        activeBattlers[battlerToHide].theSprite.GetComponent<SpriteGlowEffect>().enabled = false; // hides sprite glow        
    }

     public void AttackRolls() // creates function to manage common attack rolls algorithm
    {
        if (movesList[moveIndex].isWeapon) // executes if move is of type weapon
        {
            RollHit(); // rolls to see if attack hits

            if (attackHit) // executes if attack hit
            {
                RollEvadeBlock(); // calls function to check if attack was evaded or blocked by defender

                if (attackEvaded) // executes if attack was evaded by defender
                {
                    Instantiate(theDamageNumber, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation).SetText("Evade!"); // instantiates Evade on target
                }
                else if (attackBlocked) // executes if attack was blocked by defender
                {
                    Instantiate(theDamageNumber, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation).SetText("Block!"); // instantiates Block on target
                }
                else // executes if attack hit but was not evaded or blocked
                {
                    RollCrit(); // calls roll crit function to determine if attack crit

                    movePower = movesList[moveIndex].movePower; // pulls move power from moves list and stores to movePower variable
                    RollWeaponDamage(); // rolls weapon damage on selected target
                    DealDamage(); // calls function to deal damage to player based on selected target and previous damage roll
                }
            }

            else // executes if attack missed
            {
                Instantiate(theDamageNumber, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation).SetText("Miss"); // instantiates Miss on target
            }
        }

        else if (movesList[moveIndex].isTech) // executes if move is of type Tech
        {
            RollCrit(); // rolls to see if tech ability crits
            movePower = movesList[moveIndex].movePower; // pulls move power from moves list and stores to movePower variable
            RollTechDamage(); // rolls Tech damage on selected target
            DealDamage(); // calls function to deal damage to player
        }

        else // executes if move is of type status
        {
            RollStatusResist(); // call function to check if defender resisted status effect

            if (statusResisted) // executes if status effect was resisted
            {
                Instantiate(theDamageNumber, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation).SetText("Resisted"); // instantiates Resisted on target

            }
            else // executes if status effect was not resisted
            {
                // *** NEED TO ADD FUNCTION TO APPLY STATUS EFFECT ***
            }
        }
    }

    public void RollHit() // creates function to roll if a weapon attack hits
    {       
        int hitRoll = Mathf.RoundToInt(Random.Range(0f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        Debug.Log("Attacker hit chance = " + activeBattlers[currentTurn].hitChance); // prints attacker hit chance to debug log
        Debug.Log("Hit roll = " + hitRoll); // prints result of hit roll to debug log

        if (hitRoll <= activeBattlers[currentTurn].hitChance) // executes if attack hit
        {
            attackHit = true; // sets attackHit to true if attack hit
            Debug.Log("Attack hits."); // prints hit success to debug log
        }
        else // executes if attack missed
        {
            attackHit = false; // sets attackHit to false if attack missed
            damageRoll = 0; // sets damage roll to 0
            Debug.Log("Attack misses."); // prints hit failure to debug log
        }        
    }

    public void RollCrit() // creates function to roll if a weapon/Tech attack crits
    {
        int critChance = activeBattlers[currentTurn].critChance; // assigns local variable critChance to attacker's crit chance
        int critRoll = Mathf.RoundToInt(Random.Range(0f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        Debug.Log("Attacker crit chance = " + critChance); // prints attacker crit chance to debug log
        Debug.Log("Crit roll = " + critRoll); // prints result of crit roll

        if (critRoll <= critChance) // executes if attack crits
        {
            attackCrit = true; // sets attackCrit to true if attack crit
            critMulti = 2f; // sets crit multiplier to 2x
            Debug.Log("Attack crits."); // prints crit success notifier to debug log
        }
        else // executes if attack does not crit
        {
            attackCrit = false; // sets attackCrit to false if attack did not crit
            critMulti = 1f; // sets crit multiplier to 1x
            Debug.Log("Attack does not crit."); // print crit failure notifier to debug log
        }
    }

    public void RollEvadeBlock() // creates function to roll if a defender evades/blocks
    {
        // *** NEED TO ADD CODE TO ROLL EVADE/BLOCK ***
        // *** NEED TO HANDLE SEPARATE CASES FOR EVADE OR BLOCK, OR BOTH? ***
    }

    public void RollWeaponDamage() // creates function to roll weapon attack damage
    {
        float multiMelee = 1f, multiRanged = 1f, damageFloat = 0f; // creates float variables to handle melee and ranged damage multipliers

        // prints universal weapon damage and defense parameters to debug log
        Debug.Log("Attacker move damage = " + movePower);
        Debug.Log("Attacker weapon damage = " + activeBattlers[currentTurn].dmgWeapon);
        Debug.Log("Attacker crit multiplier = " + critMulti);
        Debug.Log("Defender weapon defense = " + activeBattlers[selectedTarget].defWeapon);


        if (!movesList[moveIndex].isRanged) // executes if weapon attack is not ranged (i.e. melee)
        {
            Debug.Log("Attack is melee."); // prints melee attack type to debug log
            multiMelee = (activeBattlers[currentTurn].strength / 2f); // calculates attacker melee multiplier  = (strength / 2)
            Debug.Log("Attacker melee multiplier = " + multiMelee); // prints melee multiplier to debug log
            damageFloat = (movePower + activeBattlers[currentTurn].dmgWeapon) * multiMelee * (100f / (100f + activeBattlers[selectedTarget].defWeapon)) * critMulti * Random.Range(0.9f, 1.1f); // calculates damage based on move power, attacker weapon and strength, crit multiplier, target weapon defense, and 10% RNG

        }
        else // executes if weapon attack is ranged
        {
            Debug.Log("Attack is ranged."); // prints ranged attack type to debug log
            multiRanged = (activeBattlers[currentTurn].agility / 2f); // calculates attacker ranged multiplier = (agility / 2)
            Debug.Log("Attacker ranged multiplier = " + multiRanged); // prints ranged multiplier to debug log
            damageFloat = (movePower + activeBattlers[currentTurn].dmgWeapon) * multiRanged * (100f / (100f + activeBattlers[selectedTarget].defWeapon)) * critMulti * Random.Range(0.9f, 1.1f); // calculates damage based on move power, attacker weapon and strength, crit multiplier, target weapon defense, and 10% RNG
        }

        // calculates rounded attack damage
        damageRoll = Mathf.RoundToInt(damageFloat); // rounds damage calc float to damage roll int

        // prints damage results to debug log
        Debug.Log("Weapon damage float = " + damageFloat); // prints damage float calc to debug log
        Debug.Log("Weapon damage roll = " + damageRoll); // prints damage roll to debug log
    }

    public void RollTechDamage() // creates function to roll tech attack damage
    {
        string elementType = ""; // creates local string variable to manage element type of attack

        // prints move and weapon base damage parameters to debug log
        Debug.Log("Attacker move damage = " + movePower);
        Debug.Log("Tech multiplier = " + activeBattlers[currentTurn].tech);
        Debug.Log("Attacker crit multiplier = " + critMulti);
        Debug.Log("Defender Tech defense = " + activeBattlers[selectedTarget].defTech);

        // checks for attack Tech element type, applies target's associated elemental resistance to resMulti, saves type in elementType string
        if(movesList[moveIndex].element == "Heat")
        {
            resMulti = activeBattlers[selectedTarget].resHeat;
            elementType = "Heat";
        }
        else if (movesList[moveIndex].element == "Freeze")
        { 
            resMulti = activeBattlers[selectedTarget].resFreeze;
            elementType = "Freeze";
        }
        else if (movesList[moveIndex].element == "Shock")
        {
            resMulti = activeBattlers[selectedTarget].resShock;
            elementType = "Shock";
        }
        else if (movesList[moveIndex].element == "Virus")
        {
            resMulti = activeBattlers[selectedTarget].resVirus;
            elementType = "Virus";
        }
        else if (movesList[moveIndex].element == "Chem")
        {
            resMulti = activeBattlers[selectedTarget].resChem;
            elementType = "Chem";
        }
        else if (movesList[moveIndex].element == "Kinetic")
        {
            resMulti = activeBattlers[selectedTarget].resKinetic;
            elementType = "Kinetic";
        }
        else if (movesList[moveIndex].element == "Water")
        {
            resMulti = activeBattlers[selectedTarget].resWater;
            elementType = "Water";
        }
        else if (movesList[moveIndex].element == "Quantum")
        {
            resMulti = activeBattlers[selectedTarget].resQuantum;
            elementType = "Water";
        }

        Debug.Log("Elemental type = " + elementType); // prints element type to debug log
        Debug.Log("Defender elemental resistance = " + resMulti + "x"); // prints defender elemental resistance multiplier

        // calculates Tech attack damage
        float damageFloat = movePower * activeBattlers[currentTurn].tech * (100f / (100f + activeBattlers[selectedTarget].defTech)) * critMulti * resMulti * Random.Range(0.9f, 1.1f); // calculates damage based on move power, attacker Tech, target Tech defense, affected elemental resistance, and 10% RNG

        // calculates rounded attack damage
        damageRoll = Mathf.RoundToInt(damageFloat); // rounds damage calc float to damage roll int

        // prints damage results to debug log
        Debug.Log("Tech damage float = " + damageFloat); // prints damage float calc to debug log
        Debug.Log("Tech damage roll = " + damageRoll); // prints damage roll to debug log
    }

    public void RollStatusResist() // creates function to roll if a defender resists a status effect
    {
        int statusRoll = Mathf.RoundToInt(Random.Range(0f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        Debug.Log("Defender status resist chance = " + activeBattlers[selectedTarget].endurance); // prints defender's status resist to debug log
        Debug.Log("Status roll = " + statusRoll); // prints result of status roll to debug log

        if (statusRoll <= activeBattlers[currentTurn].endurance) // executes if status was resisted
        {
            statusResisted = true; // sets statusResisted to true if status was resisted
            Debug.Log("Status effect resisted."); // prints resist success to debug log
        }
        else // executes if status was not resisted
        {
            statusResisted = false; // sets statusResisted to false if status was not resisted
            Debug.Log("Status effect applied."); // prints resist failure to debug log
        }
    }
}
