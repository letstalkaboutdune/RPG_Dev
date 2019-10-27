using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public BattleType[] potentialBattles; // creates BattleType array object to store various battle configurations

    public bool activateOnEnter, activateOnStay, activateOnExit; // creates boolean options to manage battle activation

    private bool inArea; // creates bool variable to check if player is in battle zone area

    public float timeBetweenBattles = 10; // creates float to handle nominal time between battles, initializes to 10 sec by default
    private float betweenBattleCounter; // creates float to handle battle time counter

    public bool deactivateAfterStarting; // creates bool to handle if battle zone should deactivate after a battle starts

    public bool cannotFlee; // creates bool to handle if cannot flee from a specific battle

    public bool shouldCompleteQuest; // creates bool to handle if battle should complete quest
    public string questToComplete; // creates string to handle if quest should complete upon battle completion

    public int musicToPlay = 0; // creates int to handle battle music to play, initializes to default of 0

    // Start is called before the first frame update
    void Start()
    {
        betweenBattleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f); // sets between battle counter to time between battles with 50% tolerance
    }

    // Update is called once per frame
    void Update()
    {
        if (inArea && PlayerController.instance.canMove) // checks if player is in the battle zone and can move
        {
            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) // checks if player is moving in either axis
            {
                betweenBattleCounter -= Time.deltaTime; // decrements battle counter
            }

            if(betweenBattleCounter <= 0) // checks if battle counter has reached 0
            {
                betweenBattleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f); // resets counter to a new initial value
                
                StartCoroutine(StartBattleCo()); // calls coroutine to start battle
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // executes if object enters collider area
    {
        if(other.tag == "Player") // checks if entering object is player
        {
            if (activateOnEnter) // checks if activate on enter option is set
            {
                StartCoroutine(StartBattleCo()); // calls start battle coroutine
            }
            else // executes if activate on enter is not true
            {
                inArea = true; // sets in area tag to true
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) // executes if object exits collider area
    {
        if (other.tag == "Player") // checks if entering object is player
        {
            if (activateOnExit) // checks if activate on exit option is set
            {
                StartCoroutine(StartBattleCo()); // calls start battle coroutine
            }
            else // executes if activate on exit is not true
            {
                inArea = false; // sets in area tag to false
            }
        }
    }

    public IEnumerator StartBattleCo() // creates coroutine to start battles
    {
        GameManager.instance.battleActive = true; // sets battle active tag to true
        UIFade.instance.FadeToBlack(); // calls UI fade to black function

        int selectedBattle = Random.Range(0, potentialBattles.Length); // creates variable to handle selected battle, picks randomly from available battles

        BattleManager.instance.rewardItems = potentialBattles[selectedBattle].rewardItems; // passes selected battle reward items to battle manager
        BattleManager.instance.rewardXP = potentialBattles[selectedBattle].rewardXP; // passes selected battle reward EXP to battle manager
        BattleManager.instance.rewardGold = potentialBattles[selectedBattle].rewardGold; // passes selected battle gold to battle manager

        yield return new WaitForSeconds(1.5f); // forces 1.5-second wait to allow for fade to black

        BattleManager.instance.BattleStart(potentialBattles[selectedBattle].enemies, cannotFlee, musicToPlay); // starts battle based on enemies from selected battle, cannotFlee flag, and music to play

        UIFade.instance.FadeFromBlack(); // calls UI fade from black function

        if (deactivateAfterStarting) // checks if battle zone should deactivate after battle starts
        {
            gameObject.SetActive(false); // deactivates battle zone game object
        }

        // passes quest marking variables to battle reward handler
        BattleReward.instance.markQuestComplete = shouldCompleteQuest;
        BattleReward.instance.questToMark = questToComplete;
    }
}
