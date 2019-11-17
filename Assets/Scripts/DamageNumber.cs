// DamageNumber
// handles the display of damage numbers in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class DamageNumber : MonoBehaviour
{
    public Text damageText; // creates text variable to handle damage text
    public string damageString; // creates string to handle damage string and color
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
        if (damageAmount > 0) // checks if damage is positive (target was hurt by attack)
        {
            damageString = "<color=white>" + damageAmount.ToString() + "</color>"; // sets damage string text to white
        }
        else if (damageAmount < 0) // checks if damage is negative (target absorbed attack)
        {
            damageAmount = Mathf.Abs(damageAmount); // removes minus sign from damage amount
            damageString = "<color=#52ff52ff>" + damageAmount.ToString() + "</color>"; // sets damage string text to green
        }
        
        if (BattleManager.instance.attackCrit) // checks if attack crit
        {
            // assigns crit message and damage number to damage text
            damageText.text = "Crit!\n";
            damageText.text += damageString; 
            //damageText.text = "Crit!\n" + damageAmount.ToString(); // assigns crit message and damage number to damage text
        }
        else // executes if attack did not crit
        {
            damageText.text = damageString;
            //damageText.text = damageAmount.ToString(); // assigns damage number to damage text
        }
        
        // sets some amount of jitter on damage text position
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0);
    }
    
    public void SetText(string textToSet) // creates function to set string of damage text, for miss/evade/block
    {
        damageText.text = textToSet; // assigns damage text based on passed string

        // sets some amount of jitter on damage text position
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0);
    }
}
