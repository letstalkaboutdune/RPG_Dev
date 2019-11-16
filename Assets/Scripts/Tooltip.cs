// Tooltip
// Manages generation/display of UI tooltips

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private Text tooltip;
        
    // Start is called before the first frame update
    void Start()
    {
        tooltip = GetComponentInChildren<Text>(); // finds text component in tooltip panel
        gameObject.SetActive(false); // hides tooltip panel by default
    }

    void Update()
    {
        // determines which corner of the screen is closest to the mouse position
        // uses a ternary conditional operator (condition ? consequent : alternative)
        Vector2 corner = new Vector2(((Input.mousePosition.x > (Screen.width / 2f)) ? 1f : 0f),
                                     ((Input.mousePosition.y > (Screen.height / 2f)) ? 1f : 0f));
        
        // sets the pivot corner of the tooltip to the opposite corner of whichever is closest
        (this.transform as RectTransform).pivot = corner;
    }

    public void GenerateTooltip(Item item) // creates a function to generate a tooltip based on item stats
    {
        // creates new strings to store stat and item text
        string statText = "";
        string itemName = item.name;
        //string itemValue = "";

        if (item.isItem) // checks if passed item is an item
        {
            // Item properties: affectHP, affectSP, affectLife, amounttoChange
            // Assigns all relevant properties to item stat text
            if (item.affectHP)
            {
                statText += "<color=red>HP +" + item.amountToChange + "</color>\n";
            }
            if (item.affectSP)
            {
                statText += "<color=cyan>SP +" + item.amountToChange + "</color>\n";
            }
            if (item.affectLife)
            {
                statText += "Revives dead player\n";
                statText += "<color=red>HP +" + item.amountToChange + "</color>\n";
            }
        }
        else if (item.isWeapon) // checks if passed item is a weapon
        {
            // Weapon properties: isRanged, isTwoHanded, dmgWeapon, hitChance, critWeapon
            // Assigns all relevant properties to stat text
            statText += "Weapon Dmg +" + item.dmgWeapon + "\n"; 
            statText += "Hit% = " + item.hitChance + "\n";
            statText += "Crit +" + item.critWeapon + "\n";
            if (item.isRanged)
            {
                statText += "Ranged\n";
            }
            else
            {
                statText += "Melee\n";
            }
            if (item.isTwoHanded)
            {
                statText += "Two-handed\n";
            }
        }
        else if (item.isArmor) // checks if passed item is armor
        {
            // Armor properties: defWeapon, defTech, evadeArmor            
            statText += "Weapon Def +" + item.defWeapon + "\n";
            statText += "Tech Def +" + item.defTech + "\n";
            statText += "Evade +" + item.evadeArmor + "\n";
        }
        else if (item.isOffHand)  // checks if passed item is an offhand
        {
            // Offhand properties: dmgWeapon, defWeapon, defTech, blockShield, hitChance, critWeapon, isRanged
            // Assigns all relevant properties to stat text            
            if(item.dmgWeapon != 0)
            {
                statText += "Weapon Dmg +" + item.dmgWeapon + "\n";
            }
            if (item.defWeapon != 0)
            {
                statText += "Weapon Def +" + item.defWeapon + "\n";
            }
            if (item.defTech != 0)
            {
                statText += "Tech Def +" + item.defTech + "\n";
            }
            if (item.blockShield != 0)
            {
                statText += "Block +" + item.blockShield + "\n";
            }
            if (item.hitChance != 0)
            {
                statText += "Hit% = " + item.hitChance + "\n";
            }
            if (item.critWeapon != 0)
            {
                statText += "Crit +" + item.critWeapon + "\n";
            }
            if (item.isRanged)
            {
                statText += "Ranged\n";
            }
        }
        else if (item.isAccy) // checks if passed item is an accessory
        {
            // creates new array to hold names of elements
            string[] elementNames = new string[] {"Heat", "Freeze", "Shock", "Virus", "Chem", "Kinetic", "Water", "Quantum"}; 
            
            // Accessory properties: resistances[]
            for (int i = 0; i < item.resistances.Length; i++) // iterates through all elements of item resistances array
            {
                if(item.resistances[i] != 1f) // checks if an element of the array is non-zero
                {
                    statText += elementNames[i] + " vulnerability = " + item.resistances[i] + "x \n"; // adds that resistance to stat text
                }
            }
        }
        else // handles any mis-typed items
        {
            statText = "Error! Item type not found in database."; // sets error message to stat text
        }

        statText = statText.Trim(); // trims extra white space off ends of stat text

        itemName = GameManager.instance.GetItemTier(itemName); // calls get item tier function to format item name color

        string tooltipText = string.Format("{0}\n{1}", itemName, statText/*, itemValue*/); // formats string of tooltip text
        tooltip.text = tooltipText; // sets tooltip text equal to formatted string
        gameObject.SetActive(true); // shows tooltip panel
    }

    public void GenerateTooltip(string stat) // creates a function to generate a tooltip based on string
    {
        string tooltipText = ""; // generates empty string to store tooltip text
        // *** NEED TO GENERATE EVENTS ON MOUSEOVER IN STATUS MENU ***
        switch (stat)
        {
            case "Row Label":
                tooltipText = "Front row gives/takes full melee damage.\nBack row gives/takes half melee damage.\nRanged and Tech damage ignores row.";
                break;
            case "Strength":
                tooltipText = "Multiplies melee damage\nMulti = Strength/2\nAffects Block%";
                break;
            case "Tech":
                tooltipText = "Multiplies Tech ability power\nMulti = Tech/2\nSets SP gain per level";
                break;
            case "Endurance":
                tooltipText = "Sets HP gain per level\nSets chance to resist statuses";
                break;
            case "Agility":
                tooltipText = "Multiplies ranged damage\nMulti = Agility/2\nAffects Crit%\nAffects Evade%";
                break;
            case "Luck":
                tooltipText = "Affects Crit%\nAffects enemy item drops";
                break;
            case "Speed":
                tooltipText = "Affects Evade%\nAffects turn order & turn rate";
                break;
            case "Weapon Dmg":
                tooltipText = "Base damage of equipped weapon\nAdds to base damage of attack skill\nMultiplied by Str/Agi multi for total damage";
                break;
            case "Hit%":
                tooltipText = "% chance to hit target when attacking\nSet by equipped weapon\nHit% = 90 when unarmed";
                break;
            case "Crit%":
                tooltipText = "% chance to deal 2x damage\nCrit% = (Agility + Luck + WeaponCrit)/3";
                break;
            case "Weapon Def":
                tooltipText = "Defense against weapon damage\nSet by equipment";
                break;
            case "Evade%":
                tooltipText = "% chance to dodge an attack\nEvade% = (Agility + Speed + ArmorEvade)/3";
                break;
            case "Block%":
                tooltipText = "% chance to block an attack\nBlock% = (Strength + ShieldBlock)/2\nBlock% = 0 with no shield equipped";
                break;
            case "Tech Def":
                tooltipText = "Defense against Tech damage\nSet by equipment";
                break;
            default:
                tooltipText = "Stat was not found!";
                break;
        }

        tooltip.text = tooltipText; // displays tooltip text
        gameObject.SetActive(true); // shows tooltip
    }

    public void GenerateTooltip(CharStats player) // creates a function to generate a tooltip based on passed player stats
    {
        // need name, level, currenthp, maxhp, currentsp, maxsp
        // creates local variables for player stats
        string playerName, playerLV, playerCurrentHP, playerMaxHP, playerCurrentSP, playerMaxSP;

        // assigns player stats to relevant strings
        playerName = player.charName;
        playerLV = player.playerLevel.ToString();
        playerCurrentHP = player.currentHP.ToString();
        playerMaxHP = player.maxHP.ToString();
        playerCurrentSP = player.currentSP.ToString();
        playerMaxSP = player.maxSP.ToString();

        string tooltipText = string.Format("{0}\n<color=#F4B913FF>LV:</color> {1}\n<color=#F4B913FF>HP:</color> {2}/{3}\n<color=#F4B913FF>SP:</color> {4}/{5}", playerName, playerLV, playerCurrentHP, playerMaxHP, playerCurrentSP, playerMaxSP); // formats string of tooltip text
        tooltip.text = tooltipText; // sets tooltip text equal to formatted string
        gameObject.SetActive(true); // shows tooltip panel
    }
}
