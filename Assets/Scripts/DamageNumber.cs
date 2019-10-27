// DamageNumber
// handles the display of damage numbers in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class DamageNumber : MonoBehaviour
{
    public Text damageText; // creates text variable to handle damage text
    public float lifetime = 1f; // creates float variable to handle damage text lifetime
    public float moveSpeed = 1f; // creates float variable to handle damage text move speed

    public float placementJitter = 0.5f; // creates float variable to handle variation of damage text location
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, lifetime); // destroys damage text after lifetime is over
        transform.position += new Vector3(0f, moveSpeed * Time.deltaTime, 0); // sets text to move up screen by move speed synced to frame rate
    }

    public void SetDamage(int damageAmount) // creates function to set number of damage text
    {
        // WIP
        if (BattleManager.instance.attackCrit)
        {
            damageText.text = "Crit!\n" + damageAmount.ToString(); // assigns crit message and damage number to damage text
        }
        else
        {
            damageText.text = damageAmount.ToString(); // assigns damage number to damage text
        }
        // END WIP        

        // sets some amount of jitter on damage text position
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0);
    }
    
    // WIP
    public void SetText(string textToSet) // creates function to set string of damage text, for miss/evade/block
    {
        damageText.text = textToSet; // assigns damage text based on passed string

        // sets some amount of jitter on damage text position
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0);
    }
    // END WIP
}
