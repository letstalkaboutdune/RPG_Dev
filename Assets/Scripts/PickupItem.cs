// PickupItem
// handles the execution of the player picking up items in the world

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private bool canPickup; // creates bool to handle whether player can pick up an item

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canPickup && Input.GetButtonDown("Fire1") & PlayerController.instance.canMove) // checks if canPickup has been set, player is pressing Fire1 and can move
        {
            GameManager.instance.AddItem(GetComponent<Item>().itemName); // uses GetComponent to find item object associated with script
                                                                         // calls add item function to add item to inventory
            Destroy(gameObject); // destroys instance of item in the world
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // runs when object enters linked collider
    {
        if(other.tag == "Player") // checks if object that collided is the player
        {
            canPickup = true; // sets canPickup bool true
        }
    }

    private void OnTriggerExit2D(Collider2D other) // runs when object exits linked collider
    {
        if (other.tag == "Player") // checks if object that collided is the player
        {
            canPickup = false; // sets canPickup bool false
        }
    }
}