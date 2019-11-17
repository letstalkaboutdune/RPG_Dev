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

    public Text[] playerName, playerHP, playerSP; // creates Text arrays to handle player stats in battle UI
    public Slider[] playerHPSlider, playerSPSlider; // creates slider arrays to display player stats in battle UI

    // creates game objects and BattleTargetButton objects to manage player target menu
    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    // creates game object and BattleMagicSelect button objects to manage player magic menu
    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;

    public BattleNotification battleNotice; // creates reference to battle notification object
    public bool battleNoticeActive; // creates bool to handle if battle notice is active

    public int chanceToFlee = 33; // creates int to handle chance to fleeing from battle
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
    public int[] itemChances;

    public bool cannotFlee; // creates bool to handle whether party can flee from battle

    public AttackEffect effectCheck; // creates AttackEffect to check name of spell effects
    public float waitCounter; // creates float to handle wait counter for attack animations

    public bool playerActing = false; // creates bool to handle if player is taking a turn

    // creates variables to manage sprite fades
    public int selectedSprite;
    public bool spriteFadeOut;

    // creates variables to manage results of attack rolls
    public bool attackHit = false, attackCrit = false, attackEvaded = false, attackBlocked = false, statusResisted = false;
    public float critMulti = 1f, resMulti = 1f;
    public float attackRowMulti = 1f, defendRowMulti = 1f;
    public int moveIndex, movePower, selectedTarget, damageRoll;

    private SpriteRenderer enemySprite, targetSprite, effectSprite; // creates sprite renderers to handle layer sorting

    private bool shouldTick = false; // creates bool to handle when counter should be ticked
    private int[] battlerCounter; // creates int array to handle counter values for each battler

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // sets instance of BattleManager to this instance
        DontDestroyOnLoad(gameObject); // prevents BattleManager object from being destroyed on scene load
    }

    // Update is called once per frame
    void Update()
    {
        if (battleActive) // checks if battle is active
        {
            if (shouldTick) // checks if shouldTick is true
            {
                if (!CheckTurn()) // checks if check turn function did not find any battlers with move
                {
                    TickCounter(); // calls function to tick down battle counter
                }
                else // executes if check turn did find battler with move
                {
                    Debug.Log("*** NEW TURN = " + activeBattlers[currentTurn] + " ***"); // prints new turn notice to debug log
                    turnWaiting = true; // sets turnWaiting flag
                }
            }

            if (turnWaiting) // checks if we are waiting on a turn
            {
                GreenSpriteGlow(currentTurn); // shows green sprite glow on current turn active battler

                if (activeBattlers[currentTurn].isPlayer) // checks if active battler is a player
                {
                    shouldTick = false; // stops battle counter

                    if (!playerActing) // checks if player is not in the middle of a move
                    {
                        uiButtonsHolder.SetActive(true); // enables UI buttons since a player has yet to move
                    }
                }
                else
                {
                    shouldTick = false; // stops battle counter
                    uiButtonsHolder.SetActive(false); // hides UI buttons since a player is inactive
                    StartCoroutine(EnemyMoveCo()); // calls EnemyMoveCo coroutine to handle enemy turn
                }
            }

            /*
            // *DEBUG ONLY* - checks for N input to test turn order
            if (Input.GetKeyDown(KeyCode.N))
            {
               NextTurn(); // calls next turn function
            } 
            // END DEBUG    
            */

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

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee, int musicToPlay, int[] enemyPlacement) // creates function to manage battle start
                                                                                                                 // requires string array of enemy names to execute
                                                                                                                 // also requires bool for whether party can flee
                                                                                                                 // also requires bool[] for enemy placement
    {       
        if (!battleActive) // checks if a battle is already underway
        {
            cannotFlee = setCannotFlee; // sets cannot flee flag to passed setCannotFlee bool
            
            battleActive = true; // sets battleActive to true to allow battle to start
            Debug.Log("*** BATTLE START ***"); // prints battle start notification to debug log

            GameManager.instance.battleActive = true; // sets battleActive in game manager to true to prevent player movement

            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z); // sets position of battle manager to current camera position
                                                                                                                                        // uses built-in Unity function of Camera.main to pull transform.position
             // sets battleScene and menu objects active to show battle UI
            battleScene.SetActive(true);
            statsHolder.SetActive(true);

            AudioManager.instance.PlayBGM(musicToPlay); // starts battle music based on passed int

            for (int i = 0; i < GameMenu.instance.activePartyList.Length; i++) // iterates through all active party list
            {
                if (GameMenu.instance.activePartyList[i] != "Empty") // checks if player slot is not empty
                {
                    //Debug.Log("Pulling stats for " + GameMenu.instance.activePartyList[i]);
                    CharStats activePlayer = GameMenu.instance.FindPlayerStats(GameMenu.instance.activePartyList[i]); // pulls reference to active player stats by name
                    if (activePlayer != null) // checks if activePlayer returned is not null
                    {
                        //Debug.Log("Found player stats for " + GameMenu.instance.activePartyList[i]); // prints player found notice to debug log
                    }
                    else // executes if active player is null
                    {
                        //Debug.Log("Player not found!"); // prints player not found notice to debug log
                    }

                    for (int j = 0; j < playerPrefabs.Length; j++) // iterates through all elements of player prefabs array
                    {
                        if (playerPrefabs[j].charName == GameMenu.instance.activePartyList[i]) // checks if active player matches a player prefab
                        {
                            if (activePlayer.inFrontRow) // checks if player is in front row
                            {
                                BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation); // instantiates new player at set position
                                newPlayer.transform.parent = playerPositions[i]; // sets new player instance as a child of player positions array
                                activeBattlers.Add(newPlayer); // adds new player element to active battlers list using built-in Add function
                                //Debug.Log("Added player to front row.");
                            }
                            else // executes if player is in the back row
                            {
                                BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i + 3].position, playerPositions[i + 3].rotation); // instantiates new player at set position in back row
                                newPlayer.transform.parent = playerPositions[i + 3]; // sets new player instance as a child of player positions array in back row
                                activeBattlers.Add(newPlayer); // adds new player element to active battlers list using built-in Add function
                                //Debug.Log("Added player to back row.");
                            }

                            // assigns all player stats for the player                            
                            activeBattlers[activeBattlers.Count - 1].inFrontRow = activePlayer.inFrontRow;
                            activeBattlers[activeBattlers.Count - 1].currentHP = activePlayer.currentHP;
                            activeBattlers[activeBattlers.Count - 1].maxHP = activePlayer.maxHP;
                            activeBattlers[activeBattlers.Count - 1].currentSP = activePlayer.currentSP;
                            activeBattlers[activeBattlers.Count - 1].maxSP = activePlayer.maxSP;
                            activeBattlers[activeBattlers.Count - 1].strength = activePlayer.strength;
                            activeBattlers[activeBattlers.Count - 1].tech = activePlayer.tech;
                            activeBattlers[activeBattlers.Count - 1].endurance = activePlayer.endurance;
                            activeBattlers[activeBattlers.Count - 1].agility = activePlayer.agility;
                            activeBattlers[activeBattlers.Count - 1].luck = activePlayer.luck;
                            activeBattlers[activeBattlers.Count - 1].speed = activePlayer.speed;
                            activeBattlers[activeBattlers.Count - 1].dmgWeapon = activePlayer.dmgWeapon;
                            activeBattlers[activeBattlers.Count - 1].hitChance = activePlayer.hitChance;
                            activeBattlers[activeBattlers.Count - 1].critWeapon = activePlayer.critWeapon;
                            activeBattlers[activeBattlers.Count - 1].critChance = activePlayer.critChance;
                            activeBattlers[activeBattlers.Count - 1].evadeArmor = activePlayer.evadeArmor;
                            activeBattlers[activeBattlers.Count - 1].evadeChance = activePlayer.evadeChance;
                            activeBattlers[activeBattlers.Count - 1].blockShield = activePlayer.blockShield;
                            activeBattlers[activeBattlers.Count - 1].blockChance = activePlayer.blockChance;
                            activeBattlers[activeBattlers.Count - 1].defWeapon = activePlayer.defWeapon;
                            activeBattlers[activeBattlers.Count - 1].defTech = activePlayer.defTech;
                            activeBattlers[activeBattlers.Count - 1].equippedWpn = activePlayer.equippedWpn;
                            activeBattlers[activeBattlers.Count - 1].equippedOff = activePlayer.equippedOff;
                            activeBattlers[activeBattlers.Count - 1].equippedArmr = activePlayer.equippedArmr;
                            activeBattlers[activeBattlers.Count - 1].equippedAccy = activePlayer.equippedAccy;
                            //Debug.Log("Imported battler stats.");
                            
                            for (int k = 0; k < activePlayer.resistances.Length; k++) // iterates through all elements of player resistances array
                            {
                                activeBattlers[activeBattlers.Count - 1].resistances[k] = activePlayer.resistances[k]; // assigns resistances from player stats to active battler                                
                            }
                            //Debug.Log("Imported battler resistances.");

                            activeBattlers[activeBattlers.Count - 1].movesAvailable = new string[activePlayer.playerAPLevel]; // initializes active battler moves list array to size equal to player AP level

                            for (int k = 0; k < activePlayer.playerAPLevel; k++) // iterates as many times as the player ability level
                            {
                                activeBattlers[activeBattlers.Count - 1].movesAvailable[k] = activePlayer.abilities[k]; // builds the player's move list based on the abilities unlocked for that ability level                                
                            }
                            //Debug.Log("Imported battler abilities.");

                            activeBattlers[activeBattlers.Count - 1].tickRate = Mathf.RoundToInt((0.2f * activeBattlers[activeBattlers.Count - 1].speed) + 5f); // assigns player tick rate based on speed
                            //Debug.Log("Imported battler tick rate.");
                        }
                    }
                }
                else // executes if active party slot is empty
                {
                    //Debug.Log("Skipping empty player slot"); // prints skip notice to debug log
                }
            }

            Debug.Log("Players instantiated successfully."); // prints players instantiated notice to debug log

            // instantiates enemies as active battlers
            for (int i = 0; i < enemiesToSpawn.Length; i++) // iterates through all elements of enemies to spawn array
            {
                if (enemiesToSpawn[i] != "") // checks if enemy in array is not blank
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++) // iterates through all elements of enemy prefabs array
                    {
                        if(enemyPrefabs[j].charName == enemiesToSpawn[i]) // checks if any enemy in prefabs matches current element of enemies to spawn array
                        {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[enemyPlacement[i]].position, enemyPositions[enemyPlacement[i]].rotation); // instantiates new enemy at set position based on passed enemyPlacement array 
                            newEnemy.tickRate = Mathf.RoundToInt((0.2f * newEnemy.speed) + 5f); // sets enemy tick rate based on speed
                            newEnemy.transform.parent = enemyPositions[enemyPlacement[i]]; // sets new enemy instance as a child of enemy positions array

                            enemySprite = newEnemy.gameObject.GetComponent<SpriteRenderer>(); // grabs sprite renderer of newEnemy
                            
                            // checks which position the enemy is placed in, sets the sorting order accordingly so more front is always on top
                            if (enemyPlacement[i] == 0 || enemyPlacement[i] == 3)
                            {
                                enemySprite.sortingOrder = 0;
                            }
                            else if (enemyPlacement[i] == 1 || enemyPlacement[i] == 4)
                            {
                                enemySprite.sortingOrder = 1;
                            }
                            else
                            {
                                enemySprite.sortingOrder = 2;
                            }

                            activeBattlers.Add(newEnemy); // adds new player element to active battlers list
                        }
                    }
                }
            }
            Debug.Log("Enemies instantiated successfully."); // prints enemies instantiated notice to debug log

            // initializes and starts battle counter ticking down
            battlerCounter = new int[activeBattlers.Count]; // initializes battleCounter array to empty int equal to number of active battlers
            StartTurnOrder(); // calls function to initialize battler turn order
            shouldTick = true; // starts battle counter

            AddSpriteGlow(); // calls function to add sprite glow to all active battlers
            UpdateBattle(); // calls function to update battle to reflect player dead upon battle entry
            UpdateUIStats(); // calls function to update UI battle stats
        }
    }

    public void NextTurn() // creates function to handle next turn in combat
    {        
        //Debug.Log("*** NEXT TURN ***"); // prints next turn notification to debug log

        HideSpriteGlow(currentTurn); // hides sprite glow from current turn battler
        UpdateBattle(); // updates information in battle whenever a turn completes
        UpdateUIStats(); // calls function to update UI battle stats

        shouldTick = true; // starts battle counter
    }

    public void UpdateGameManager() // creates function to handle updating game manager based on battle stats
    {
        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active players
        {
            if (activeBattlers[i].isPlayer) // checks if active battler is player
            {
                CharStats activePlayer = GameMenu.instance.FindPlayerStats(activeBattlers[i].charName); // pulls reference to player stats
                activePlayer.currentHP = activeBattlers[i].currentHP;
                activePlayer.maxHP = activeBattlers[i].maxHP;
                activePlayer.currentSP = activeBattlers[i].currentSP;
                activePlayer.maxSP = activeBattlers[i].maxSP;

                /*
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++) // iterates through all elements of playerStats array in GameManager
                {
                    if (activeBattlers[i].charName == GameManager.instance.playerStats[j].charName) // checks if active battler name matches char name in game manager
                    {
                        // updates current player stats index in game manager with active battler
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].maxHP = activeBattlers[i].maxHP;
                        GameManager.instance.playerStats[j].currentSP = activeBattlers[i].currentSP;
                        GameManager.instance.playerStats[j].maxSP = activeBattlers[i].maxSP;                        
                    }
                }
                */
            }
        }
    }

    public void UpdateBattle() // creates function to handle updating information in battle
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
        
        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation); // instantiates attacker white particle effect
        
        for (int i = 0; i < movesList.Length; i++) // iterates through all moves in moves list
        {
            if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack]) // checks if selected move is contained in move list
            {
                moveIndex = i; // assigns current value of i to moveIndex for use in damage rolls
                battlerCounter[currentTurn] = Mathf.RoundToInt(movesList[i].moveCounter * Random.Range(0.9f, 1.1f)); // resets counter on current battler to moveCounter value ±10% RNG

                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation); // instantiates attack effect on selected target

                // sets the sprite layer order of the move on top of the selected target
                targetSprite = activeBattlers[selectedTarget].gameObject.GetComponent<SpriteRenderer>(); // finds reference to selected target sprite
                effectSprite = GameObject.FindWithTag("Effects").GetComponent<SpriteRenderer>(); // finds reference to the attack effect sprite
                effectSprite.sortingOrder = (targetSprite.sortingOrder + 1); // sets attack effect sprite layer one layer above target

                yield return new WaitForSeconds(movesList[i].theEffect.effectLength); // wait for length of time so that attack animation can finish

                AttackRolls(); // calls function to manage various attack rolls
            }
        }

        yield return new WaitForSeconds(1f); // forces a one-second wait

        HideSpriteGlow(selectedTarget); // hides sprite glow on current selected target
        
        NextTurn(); // calls next turn function
    }    

    public IEnumerator PlayerMoveCo(string moveName, int playerTarget) // creates function to handle players attacking
    {
        turnWaiting = false; // sets turn waiting to false since player is doing a move
        
        selectedTarget = playerTarget;
        playerActing = true; // sets playerActing true to stop update loop from acting on UI
        
        uiButtonsHolder.SetActive(false); // hides action buttons once player action starts
        targetMenu.transform.localScale = new Vector3(0, 0, 0); // adjusts scale of targetMenu to 0 to prevent multiple clicks
        
        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation); // instantiates attacker white particle effect
        
        for (int i = 0; i < movesList.Length; i++) // iterates through all moves in moves list
        {            
            if (movesList[i].moveName == moveName) // checks if selected move is contained in move list
            {
                moveIndex = i; // assign i value to moveIndex when move is found
                battlerCounter[currentTurn] = Mathf.RoundToInt(movesList[i].moveCounter * Random.Range(0.9f, 1.1f)); // resets counter on current battler to moveCounter value ±10% RNG

                RedSpriteGlow(selectedTarget); // shows red sprite glow on current selected target
                
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation); // instantiates attack effect on selected target

                // sets the sprite layer order of the move on top of the selected target
                targetSprite = activeBattlers[selectedTarget].gameObject.GetComponent<SpriteRenderer>(); // finds reference to selected target sprite
                effectSprite = GameObject.FindWithTag("Effects").GetComponent<SpriteRenderer>(); // finds reference to the attack effect sprite
                effectSprite.sortingOrder = (targetSprite.sortingOrder + 1); // sets attack effect sprite layer one layer above target

                movePower = movesList[i].movePower; // pulls move power from moves list and stores to movePower variable                               
            }
        }

        //Debug.Log("Starting wait = " + movesList[moveIndex].theEffect.effectLength); // print wait time to debug log        
        yield return new WaitForSeconds(movesList[moveIndex].theEffect.effectLength); // wait for length of time so that attack animation can finish

        AttackRolls(); // calls function to manage various attack rolls

        //Debug.Log("Dealing damage."); // print status to debug log        
        
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
        //Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageRoll + " damage to " + activeBattlers[selectedTarget].charName);

        // subtracts target's current HP by calculated damage
        activeBattlers[selectedTarget].currentHP -= damageRoll;
        if (activeBattlers[selectedTarget].currentHP > activeBattlers[selectedTarget].maxHP) // checks if target's HP > max HP due to absorbing an attack
        {
            activeBattlers[selectedTarget].currentHP = activeBattlers[selectedTarget].maxHP; // clamps target's HP to max HP
        }

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

                    playerName[i].transform.localScale = new Vector3(1, 1, 1); // shows player name object if a player

                    // updates player name, HP, and SP on UI, clamps HP and SP to never be < 0
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerHPSlider[i].maxValue = playerData.maxHP;
                    playerHPSlider[i].value = playerData.currentHP;
                    playerSP[i].text = Mathf.Clamp(playerData.currentSP, 0, int.MaxValue) + "/" + playerData.maxSP;
                    playerSPSlider[i].maxValue = playerData.maxSP;
                    playerSPSlider[i].value = playerData.currentSP;
                }
                else 
                {
                    playerName[i].transform.localScale = new Vector3(0, 0, 0); // hides player name object if a player
                }
            } 
            else
            {
                playerName[i].transform.localScale = new Vector3(0, 0, 0); // hides player name object if a player
            }            
        }
    }

    public void MeleeOrRanged() // creates function to handle if player attack is melee or ranged
    {
        string weaponName = activeBattlers[currentTurn].equippedWpn; // pulls name of active player weapon
        //Debug.Log("Player weapon = " + weaponName); // prints player weapon name to debug log

        if (weaponName != "" && GameManager.instance.GetItemDetails(weaponName).isRanged) // checks if player weapon is not blank and is a ranged weapon
        {
            //Debug.Log("Rolling ranged attack."); // prints ranged attack roll to debug menu
            OpenTargetMenu("Ranged Attack"); // calls open target menu with ranged attack
        }

        else // executes if weapon is not ranged (i.e. melee)
        {
            //Debug.Log("Rolling melee attack."); // prints melee attack roll to debug menu
            OpenTargetMenu("Melee Attack"); // calls open target menu with melee attack
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
            //Debug.Log("Chance to flee = " + chanceToFlee + "%"); // prints chance to flee to debug log
            //Debug.Log("Flee roll = " + fleeSuccess); // prints flee roll to debug log

            if (fleeSuccess < chanceToFlee) // checks if flee success result is lower than threshold set by chance to flee
                                            // flee is successful when fleeSuccess < chanceToFlee
            {
                //Debug.Log("Flee successful."); // prints flee success notice to debug log
                fleeing = true; // sets fleeing boolean to true

                chanceToFlee -= 25; // increases flee difficulty by 25% on successful flee
                if(chanceToFlee < 1) // checks if chance to flee has decreased below 1
                {
                    chanceToFlee = 1; // clamps chance to flee to 1
                }

                HideSpriteGlow(currentTurn); // hides sprite glow from current turn active battler

                StartCoroutine(EndBattleCo()); // calls end battle coroutine
            }
            else
            {
                //Debug.Log("Flee failed."); // prints flee failure notice to debug log

                chanceToFlee += 25; // decreases flee difficulty by 25% on unsuccessful flee
                if (chanceToFlee > 99) // checks if chance to flee has increased above 99
                {
                    chanceToFlee = 99; // clamps chance to flee to 99
                }
                
                for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers
                {
                    if(activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0) // checks if active battler is a player and alive
                    {
                        battlerCounter[i] = Mathf.RoundToInt(100 * Random.Range(0.9f, 1.1f)); // resets counter on current battler to 100 ±10% RNG
                    }
                }
                
                turnWaiting = false; // sets turn waiting flag to false                 
                NextTurn(); // calls next turn function
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

            for (int i = 0; i < GameMenu.instance.activePartyList.Length; i++) // iterates through active party
            {
                if (GameMenu.instance.activePartyList[i] != "Empty") // checks if slot is not empty
                {
                    battleItemCharChoiceNames[i].text = GameMenu.instance.activePartyList[i]; // sets character button based on active party list
                    battleItemCharChoiceNames[i].transform.parent.transform.localScale = new Vector3(1, 1, 1); // shows char choice button    
                }
                else // executes if slot is empty
                {
                    battleItemCharChoiceNames[i].text = "Empty"; // sets character button to Empty
                    battleItemCharChoiceNames[i].transform.parent.transform.localScale = new Vector3(0, 0, 0); // adjusts scale of char choice button to 0 to hide without affecting layout group
                }
            }
        }
    }
    
    public void UseBattleItem(int selectChar) // creates function to handle use of item character choice menu buttons
    {
        battleNoticeActive = false; // resets battle notice active to false

        string charName = GameMenu.instance.activePartyList[selectChar]; // pulls name of selected char from active party list

        //Debug.Log("Batt.Use passed item = " + battleActiveItem.itemName); // prints item name to debug log

        battleActiveItem.Use(charName); // calls Use function to handle use of item on selected char

        if (battleNoticeActive)
        {
            battleNotice.Activate();
            battleNoticeActive = false;
        }
        else
        {
            //Debug.Log(battleActiveItem.itemName + " was used."); // prints debug text to notify on item use
            battlerCounter[currentTurn] = Mathf.RoundToInt(100 * Random.Range(0.9f, 1.1f)); // resets counter on current battler to 100 ±10% RNG

            CloseItemsMenu(); // calls function to close items menu
            CloseActionButtons(); // closes item action panel

            battleActiveItem = null; // resets active item to null

            // resets item details in menu to default values
            battleItemName.text = "Please select an item.";
            battleItemDescription.text = "";

            UpdateBattle(); // calls extra battle stats update, since item acts on game manager
            //Debug.Log("Calling next turn function.");
            turnWaiting = false; // sets turn waiting false to allow next turn to tick
            NextTurn(); // moves to next turn in battle
        }
    }

    public void CloseBattleItemCharChoice() // creates function to close item character choice menu
    {
        battleItemCharChoiceMenu.SetActive(false); // de-activates item character choice menu
    }

    public IEnumerator EndBattleCo() // creates IEnumerator coroutine to end battle in victory
    {
        battleActive = false; // sets battle active to false to prevent more turns
        turnWaiting = false; // sets turn waiting to false to prevent issues loading battle
        shouldTick = false; // stops battle counter

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
            BattleReward.instance.OpenRewardScreen(rewardXP, rewardAP, rewardGold, rewardItems, itemChances); // calls battle reward function based on passed XP, AP, items, and gold
        }

        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay); // changes BGM back to current scene
    }

    public IEnumerator GameOverCo() // creates IEnumerator coroutine to end battle in game over
    {
        battleActive = false; // sets battle active to false to prevent more turns
        turnWaiting = false; // sets turn waiting to false to prevent issues loading battle
        shouldTick = false; // stops battle counter

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
                // *** CREATE "CURRENT" STATS ON TOP OF BASE STATS? ***
            }
        }
    }

    public void RollHit() // creates function to roll if a weapon attack hits
    {       
        int hitRoll = Mathf.RoundToInt(Random.Range(1f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        //Debug.Log("Attacker hit chance = " + activeBattlers[currentTurn].hitChance); // prints attacker hit chance to debug log
        //Debug.Log("Hit roll = " + hitRoll); // prints result of hit roll to debug log

        if (hitRoll <= activeBattlers[currentTurn].hitChance) // executes if attack hit
        {
            attackHit = true; // sets attackHit to true if attack hit
            //Debug.Log("Attack hits."); // prints hit success to debug log
        }
        else // executes if attack missed
        {
            attackHit = false; // sets attackHit to false if attack missed
            damageRoll = 0; // sets damage roll to 0
            //Debug.Log("Attack misses."); // prints hit failure to debug log
        }        
    }

    public void RollCrit() // creates function to roll if a weapon/Tech attack crits
    {
        int critChance = activeBattlers[currentTurn].critChance; // assigns local variable critChance to attacker's crit chance
        int critRoll = Mathf.RoundToInt(Random.Range(1f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        //Debug.Log("Attacker crit chance = " + critChance); // prints attacker crit chance to debug log
        //Debug.Log("Crit roll = " + critRoll); // prints result of crit roll

        if (critRoll <= critChance) // executes if attack crits
        {
            attackCrit = true; // sets attackCrit to true if attack crit
            critMulti = 2f; // sets crit multiplier to 2x
            //Debug.Log("Attack crits."); // prints crit success notifier to debug log
        }
        else // executes if attack does not crit
        {
            attackCrit = false; // sets attackCrit to false if attack did not crit
            critMulti = 1f; // sets crit multiplier to 1x
            //Debug.Log("Attack does not crit."); // print crit failure notifier to debug log
        }
    }

    public void RollEvadeBlock() // creates function to roll if a defender evades/blocks
    {
        // code below handles evade
        // ************************
        int evadeChance = activeBattlers[selectedTarget].evadeChance; // assigns local variable to defender's evade chance
        int evadeRoll = Mathf.RoundToInt(Random.Range(1f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        //Debug.Log("Defender evade chance = " + evadeChance); // prints defender evade chance to debug log
        //Debug.Log("Evade roll = " + evadeRoll); // prints result of evade roll

        if (evadeRoll <= evadeChance) // executes if attack is evaded
        {
            attackEvaded = true; // sets attackEvaded to true if attack was evaded
            //Debug.Log("Attack evaded."); // prints evade success notifier to debug log
        }
        else // executes if attack was not evaded
        {
            attackEvaded = false; // sets attackEvaded to false if attack was not evaded
            //Debug.Log("Attack not evaded."); // prints evade failure notifier to debug log
        }
        
        // code below handles block
        // ************************
        int blockChance = activeBattlers[selectedTarget].blockChance; // assigns local variable to defender's block chance
        int blockRoll = Mathf.RoundToInt(Random.Range(1f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        //Debug.Log("Defender block chance = " + blockChance); // prints defender block chance to debug log
        //Debug.Log("Block roll = " + evadeRoll); // prints result of block roll

        if (blockRoll <= blockChance) // executes if attack is blocked
        {
            attackBlocked = true; // sets attackBlocked to true if attack was blocked
            //Debug.Log("Attack blocked."); // prints block success notifier to debug log
        }
        else // executes if attack was not blocked
        {
            attackBlocked = false; // sets attackBlocked to false if attack was not blocked
            //Debug.Log("Attack not blocked."); // prints block failure notifier to debug log
        }
    }

    public void RollWeaponDamage() // creates function to roll weapon attack damage
    {
        CheckBattlerRow(); // calls function to check row of enemy target
        
        float multiMelee, multiRanged, damageFloat; // creates float variables to handle melee and ranged damage multipliers
        //float weaponScalar = 1f, techScalar = 1f; // creates float variables to handle scale factors for damage calculation

        // prints universal weapon damage and defense parameters to debug log
        //Debug.Log("Attacker move damage = " + movePower);
        //Debug.Log("Attacker weapon damage = " + activeBattlers[currentTurn].dmgWeapon);
        //Debug.Log("Attacker crit multiplier = " + critMulti);
        //Debug.Log("Defender weapon defense = " + activeBattlers[selectedTarget].defWeapon);

        if (!movesList[moveIndex].isRanged) // executes if weapon attack is not ranged (i.e. melee)
        {
            //Debug.Log("Attack is melee."); // prints melee attack type to debug log
            multiMelee = (activeBattlers[currentTurn].strength / 2f); // calculates attacker melee multiplier  = (strength / 2)
            //Debug.Log("Attacker melee multiplier = " + multiMelee); // prints melee multiplier to debug log
            damageFloat = (movePower + activeBattlers[currentTurn].dmgWeapon) * multiMelee * attackRowMulti * defendRowMulti * (100f / (100f + activeBattlers[selectedTarget].defWeapon)) * critMulti * Random.Range(0.9f, 1.1f); // calculates damage based on move power, attacker weapon and strength, crit multiplier, target weapon defense, and 10% RNG

        }
        else // executes if weapon attack is ranged
        {
            //Debug.Log("Attack is ranged."); // prints ranged attack type to debug log
            multiRanged = (activeBattlers[currentTurn].agility / 2f); // calculates attacker ranged multiplier = (agility / 2)
            //Debug.Log("Attacker ranged multiplier = " + multiRanged); // prints ranged multiplier to debug log
            damageFloat = (movePower + activeBattlers[currentTurn].dmgWeapon) * multiRanged * (100f / (100f + activeBattlers[selectedTarget].defWeapon)) * critMulti * Random.Range(0.9f, 1.1f); // calculates damage based on move power, attacker weapon and strength, crit multiplier, target weapon defense, and 10% RNG
        }

        // calculates rounded attack damage
        damageRoll = Mathf.RoundToInt(damageFloat); // rounds damage calc float to damage roll int

        // prints damage results to debug log
        //Debug.Log("Weapon damage float = " + damageFloat); // prints damage float calc to debug log
        //Debug.Log("Weapon damage roll = " + damageRoll); // prints damage roll to debug log
    }

    public void RollTechDamage() // creates function to roll tech attack damage
    {
        //string elementType = ""; // creates local string variable to manage element type of attack

        // prints move and weapon base damage parameters to debug log
        //Debug.Log("Attacker move damage = " + movePower);
        //Debug.Log("Tech multiplier = " + activeBattlers[currentTurn].tech);
        //Debug.Log("Attacker crit multiplier = " + critMulti);
        //Debug.Log("Defender Tech defense = " + activeBattlers[selectedTarget].defTech);

        // checks for attack Tech element type, applies target's associated elemental resistance to resMulti, saves type in elementType string
        if(movesList[moveIndex].element == "Heat")
        {
            resMulti = activeBattlers[selectedTarget].resistances[0];
            //elementType = "Heat";
        }
        else if (movesList[moveIndex].element == "Freeze")
        {
            resMulti = activeBattlers[selectedTarget].resistances[1];
            //elementType = "Freeze";
        }
        else if (movesList[moveIndex].element == "Shock")
        {
            resMulti = activeBattlers[selectedTarget].resistances[2];
            //elementType = "Shock";
        }
        else if (movesList[moveIndex].element == "Virus")
        {
            resMulti = activeBattlers[selectedTarget].resistances[3];
            //elementType = "Virus";
        }
        else if (movesList[moveIndex].element == "Chem")
        {
            resMulti = activeBattlers[selectedTarget].resistances[4];
            //elementType = "Chem";
        }
        else if (movesList[moveIndex].element == "Kinetic")
        {
            resMulti = activeBattlers[selectedTarget].resistances[5];
            //elementType = "Kinetic";
        }
        else if (movesList[moveIndex].element == "Water")
        {
            resMulti = activeBattlers[selectedTarget].resistances[6];
            //elementType = "Water";
        }
        else if (movesList[moveIndex].element == "Quantum")
        {
            resMulti = activeBattlers[selectedTarget].resistances[7];
            //elementType = "Quantum";
        }

        //Debug.Log("Elemental type = " + elementType); // prints element type to debug log
        //Debug.Log("Defender elemental resistance = " + resMulti + "x"); // prints defender elemental resistance multiplier

        // calculates Tech attack damage
        float damageFloat = movePower * (activeBattlers[currentTurn].tech/2f) * (100f / (100f + activeBattlers[selectedTarget].defTech)) * critMulti * resMulti * Random.Range(0.9f, 1.1f); // calculates damage based on move power, attacker Tech, target Tech defense, affected elemental resistance, and 10% RNG

        // calculates rounded attack damage
        damageRoll = Mathf.RoundToInt(damageFloat); // rounds damage calc float to damage roll int

        // prints damage results to debug log
        //Debug.Log("Tech damage float = " + damageFloat); // prints damage float calc to debug log
        //Debug.Log("Tech damage roll = " + damageRoll); // prints damage roll to debug log
    }

    public void RollStatusResist() // creates function to roll if a defender resists a status effect
    {
        int statusRoll = Mathf.RoundToInt(Random.Range(0f, 100f)); // rolls random number between 1 and 100, rounds to nearest int
        //Debug.Log("Defender status resist chance = " + activeBattlers[selectedTarget].endurance); // prints defender's status resist to debug log
        //Debug.Log("Status roll = " + statusRoll); // prints result of status roll to debug log

        if (statusRoll <= activeBattlers[currentTurn].endurance) // executes if status was resisted
        {
            statusResisted = true; // sets statusResisted to true if status was resisted
            //Debug.Log("Status effect resisted."); // prints resist success to debug log
        }
        else // executes if status was not resisted
        {
            statusResisted = false; // sets statusResisted to false if status was not resisted
            //Debug.Log("Status effect applied."); // prints resist failure to debug log
        }
    }

    public void CheckBattlerRow() // creates function to check row of active battlers
    {
        // creates strings to store battler positions for easier access
        string atkPos = activeBattlers[currentTurn].transform.parent.name; 
        string defPos = activeBattlers[selectedTarget].transform.parent.name;
        
        // prints positions to debug log
        //Debug.Log("Active battler position = " + atkPos);
        //Debug.Log("Selected target position = " + defPos);

        // section below checks for attacker's row
        // ***************************************
        if (activeBattlers[currentTurn].isPlayer) // checks if attacker is player
        {
            if (atkPos == "Pos1_front" || atkPos == "Pos2_front" || atkPos == "Pos3_front") // checks if player is in any front position
            {
                //Debug.Log("Player attacker is in front row."); // prints player attacker row status to debug log
                attackRowMulti = 1f; // sets attack row multiplier to 1
            }
            else // executes if player is in any back position
            {
                //Debug.Log("Player attacker is in back row."); // prints player attacker row status to debug log
                attackRowMulti = 0.5f; // sets attack row multiplier to 0.5f
            }
        }
        else // executes if attacker is enemy
        {
            if (atkPos == "Pos1" || atkPos == "Pos2" || atkPos == "Pos3") // checks if enemy is in any front position
            {
                //Debug.Log("Enemy attacker is in front row."); // prints enemy attacker row status to debug log
                attackRowMulti = 1f; // sets attack row multiplier to 1
            }
            // checks if any enemies are active in front row by seeing if any children of any front row enemy position indexes are active
            else if (enemyPositions[0].GetChild(enemyPositions[0].childCount - 1).gameObject.activeInHierarchy || enemyPositions[1].GetChild(enemyPositions[1].childCount - 1).gameObject.activeInHierarchy || enemyPositions[2].GetChild(enemyPositions[2].childCount - 1).gameObject.activeInHierarchy)
            {
                //Debug.Log("Enemy attacker is in back row."); // prints enemy attacker row status to debug log
                attackRowMulti = 0.5f; // sets attack row multiplier to 0.5
            }
            else // executes if enemy is in back row but no enemies are in front row
            {
                //Debug.Log("Enemy attacker is in back row, but unprotected."); // prints enemy attacker row status to debug log
                attackRowMulti = 1f; // sets attack row multiplier to 1
            }
        }

        // section below checks for defender's row
        // ***************************************
        if (activeBattlers[selectedTarget].isPlayer) // checks if defender is player
        {
            if (defPos == "Pos1_front" || defPos == "Pos2_front" || defPos == "Pos3_front") // checks if player is in any front position
            {
                //Debug.Log("Player defender is in front row."); // prints player defender row status to debug log
                defendRowMulti = 1f; // sets defend row multiplier to 1
            }
            else // executes if player is in any back position
            {
                //Debug.Log("Player defender is in back row."); // prints player defender row status to debug log
                defendRowMulti = 0.5f; // sets defend row multiplier to 0.5f
            }
        }
        else // executes if defender is enemy
        {
            if (defPos == "Pos1" || defPos == "Pos2" || defPos == "Pos3") // checks if enemy is in any front position
            {
                //Debug.Log("Enemy defender is in front row."); // prints enemy defender row status to debug log
                defendRowMulti = 1f; // sets defend row multiplier to 1
            }
            // checks if any enemies are active in front row by seeing if any children of any front row enemy position indexes are active
            else if (enemyPositions[0].GetChild(enemyPositions[0].childCount-1).gameObject.activeInHierarchy || enemyPositions[1].GetChild(enemyPositions[1].childCount - 1).gameObject.activeInHierarchy || enemyPositions[2].GetChild(enemyPositions[2].childCount - 1).gameObject.activeInHierarchy) 
            {
                //Debug.Log("Enemy defender is in back row."); // prints enemy defender row status to debug log
                defendRowMulti = 0.5f; // sets defender row multiplier to 0.5
            }
            else // executes if enemy is in back row but no enemies are in front row
            {
                //Debug.Log("Enemy defender is in back row, but unprotected."); // prints enemy defender row status to debug log
                defendRowMulti = 1f; // sets defend row multiplier to 1
            }
        }
    }

    // TURN ORDER ALGORITHM
    // ********************
    // IN BATTLESTART, INITIALIZE ALL BATTLERS TO ICV = 100 AND SET SHOULDTICK = TRUE
    // EACH UPDATE, CHECK IF SHOULDTICK = TRUE
        // IF SO, CALL CHECKTURN
            // IF CHECKTURN FINDS BATTLER NEEDS TO MOVE, SET THAT BATTLER AS CURRENT TURN
                // IF PLAYER, SHOW THE UI BUTTONS AND SET SHOULDTICK = FALSE
                    // ONCE PLAYER MOVE COMPLETE, SET SHOULDTICK = TRUE
                // IF ENEMY, START ENEMY MOVE AND SET SHOULDTICK = FALSE
                    // ONCE ENEMY MOVE COMPLETE, SET SHOULDTICK = TRUE
            // IF CHECKTURN FINDS NO BATTLER TO MOVE, CALL TICKCOUNTER

    public void StartTurnOrder() // creates function to initialize battler turn order
    {
        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers
        {
            battlerCounter[i] = Mathf.RoundToInt(100 * Random.Range(0.9f, 1.1f)); // initializes battler counter to default of 100 ±10% RNG
        } 
    }

    public bool CheckTurn() // creates function to check for next battler turn
    {
        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers
        {
            //Debug.Log("Checking active battler " + i);
            if (activeBattlers[i].currentHP > 0) // checks if current battler is alive
            {
                if (battlerCounter[i] == 0) // checks for the first active battler with a counter = 0
                {
                    //battlerCounter[i] = Mathf.RoundToInt(100 * Random.Range(0.9f, 1.1f)); // resets counter on current battler to 100 ±10% RNG
                    currentTurn = i; // sets current turn to that battler
                    //Debug.Log("Setting current turn to " + i);
                    return true; // once battler turn found, return true
                }
            }
        }

        return false; // if no battlers turn found, return false
    }

    public void TickCounter() // creates function to decrement battler counter
    {
        for (int i = 0; i < activeBattlers.Count; i++) // iterates through all active battlers
        {
            if(activeBattlers[i].currentHP > 0) // checks if current battler is alive
            {
                //Debug.Log("Battler name = " + activeBattlers[i].charName); // prints battler name to debug log
                //Debug.Log("Battler speed = " + activeBattlers[i].speed); // prints battler speed to debug log
                //Debug.Log("Battler tick rate = " + activeBattlers[i].tickRate); // prints battler tick rate to debug log
                //Debug.Log("Battler " + i + " counter = " + battlerCounter[i]); // prints battler counter to debug log

                battlerCounter[i] -= activeBattlers[i].tickRate; // decrements counter by battler tick rate

                if (battlerCounter[i] <= 0) // checks if any battlerCounter value <= 0
                {
                    battlerCounter[i] = 0; // clamps battlerCounter value to 0
                }
            }
        }
    }

    public int FindPlayerBattlerIndex(string charName) // creates function to find active battler player index by name
    {
        int index = -1; // initializes local index to -1

        for (int i = 0; i < activeBattlers.Count; i++) // iterates through active battlers
        {
            if (activeBattlers[i].name.Contains(charName)) // checks if active battler name contains passed name
            {
                //Debug.Log("Found match for " + charName); // prints match found notice to debug log
                index = i; // sets local index to current iteration
                break; // breaks for loop once match found
            }
        }

        return index; // returns player index once function ends
    }
}
