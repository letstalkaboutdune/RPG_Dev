// BattleChar
// handles the definition of battle character object and all needed battle stats

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleChar : MonoBehaviour
{
    public bool isPlayer, hasDied; // creates bool to check if battle char is player or has died

    public string[] movesAvailable; // creats string array to handle abilities available to battle char

    public string charName; // creates string to handle battle char name

    // creates various variables to handle all battle stats
    public int currentHP, maxHP, currentMP, maxMP, strength, tech, endurance, agility, luck, speed;
    public bool inFrontRow;
    public int dmgWeapon, hitChance, critChance, evadeChance, blockChance, defWeapon, defTech;
    public float resHeat = 1f, resFreeze = 1f, resShock = 1f, resVirus = 1f, resChem = 1f, resKinetic = 1f, resWater = 1f, resQuantum = 1f;
    public string equippedWpn, equippedArmr, equippedAccy;
    
    public SpriteRenderer theSprite; // creates SpriteRenderer object to handle different sprites based on player status
    public Sprite deadSprite, aliveSprite; // creates two sprites to handle player dead/alive status

    // creates variables to handle enemy fade on death
    private bool shouldFade;
    public float fadeSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFade) // checks every frame if should fade tag is true
        {
            // sets sprite to fade to red and transparent over one second, independent of frame rate
            theSprite.color = new Color(Mathf.MoveTowards(theSprite.color.r, 1f, fadeSpeed * Time.deltaTime), Mathf.MoveTowards(theSprite.color.g, 0f, fadeSpeed * Time.deltaTime), Mathf.MoveTowards(theSprite.color.b, 0f, fadeSpeed * Time.deltaTime), Mathf.MoveTowards(theSprite.color.a, 0f, fadeSpeed * Time.deltaTime));
            
            if(theSprite.color.a == 0) // checks if enemy sprite has completed fade out
            {
                gameObject.SetActive(false); // de-activates enemy game object
            }
        }
    }

    public void EnemyFade() // creates function to control enemy fade on death
    {
        shouldFade = true; // sets should fade tag to true
    }
}