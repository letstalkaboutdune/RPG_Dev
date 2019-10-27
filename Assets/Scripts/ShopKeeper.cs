// ShopKeeper
// handles definition of items for sale at shop, detection if player can open shop window

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    private bool canOpen; // creates bool variable to handle if shop can open or not

    public string[] ItemsForSale = new string[40]; // creates string array to handle the list of items for sale in a shop
                                                   // initializes itemsForSale string array to length of 40

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canOpen && Input.GetButtonDown("Fire1") && PlayerController.instance.canMove && !Shop.instance.shopMenu.activeInHierarchy) // checks if player has collided with shop keeper, is pressing Fire1, can already move, and shop not already open
        {
            Shop.instance.itemsForSale = ItemsForSale; // sets shop items for sale based on shopkeeper's associated item names

            Shop.instance.OpenShop(); // opens shop menu
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // runs when object enters linked collider
    {
        if (other.tag == "Player") // checks if object that collided is the player
        {
            canOpen = true; // sets canOpen bool true
        }
    }

    private void OnTriggerExit2D(Collider2D other) // runs when object exits linked collider
    {
        if (other.tag == "Player") // checks if object that collided is the player
        {
            canOpen = false; // sets canOpen bool false
        }
    }
}