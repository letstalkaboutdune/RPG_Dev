// WorldMap
// handles display of world map world label UI and movement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldMap : MonoBehaviour
{
    // creates variables to manage world map
    public Text worldName;
    public string worldNameString;

    // Start is called before the first frame update
    void Start()
    {
        worldName.text = SceneManager.GetActiveScene().name + ":\n" + worldNameString; // builds world name from active scene and passed world name
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
