// BattleMove
// handles data for different battle enemy and reward configurations

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleType
{
    // creates various variables to handle elements of battle configuration
    public string[] enemies;
    public int[] enemyPlacement;
    public int rewardXP;
    public int rewardAP;
    public string[] rewardItems;
    public int rewardGold;
}
