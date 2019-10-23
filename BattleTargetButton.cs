// BattleTargetButton
// handles the function of player target button in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // includes Unity UI library

public class BattleTargetButton : MonoBehaviour
{
    // creates variables to handle player move and target selection
    public string moveName;
    public int activeBattlerTarget;
    public Text targetName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press() // handles pressing of target button after attack button
    {
        StartCoroutine(BattleManager.instance.PlayerMoveCo(moveName, activeBattlerTarget)); // calls player attack coroutine in battle manager
    }
}
