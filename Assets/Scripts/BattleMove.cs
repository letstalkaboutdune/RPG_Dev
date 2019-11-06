// BattleMove
// handles data for move name, power, cost, and effect of all abilities in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleMove
{
    public string moveName; // creates string to handle move name
    public int movePower, moveCost, moveCounter; // creates ints to handle move power, move cost, and move tick counter
    public AttackEffect theEffect; // creates attack effect object to handle attack effects
    public bool isWeapon, isTech, isStatus, isRanged; // creates bool to store various move logic properties
    public string element = ""; // creates string to store element of Tech attacks
    }
