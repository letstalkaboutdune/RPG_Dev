﻿// PlayerController
// handles player movement, defining camera limits, and area transitions

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Rigidbody2D theRB; // creates Rigidbody2D variable which is attached to player and handles physics, including movement

    public float moveSpeed; // creates float variable to handle player move speed

    public Animator myAnim; // creates Animator variable which is attached to player animator

    public static PlayerController instance; // creates reference to this particular instance of this PlayerController class/script
                                             // "static" property only allows one version of this instance for every script in the project
    public string areaTransitionName; // creates string variable to handle area transition name

    private Vector3 bottomLeftLimit, topRightLimit; // creates Vector3 variables to handle bottom-left and top-right limit of map

    public bool canMove = true;     // creates bool variable to handle restriction of player movement, sets default to true

    // Start is called before the first frame update
    //void Awake()
    void Start()
    {
        if(instance == null) // checks if PlayerController instance is null, i.e. not yet set if the game just started running
        {
            instance = this; // sets PlayerController instance to this instance
                             // "this" keyword refers to current instance of the class
        }
        else
        {
            if(instance != this) // checks if instance is not equal to this, to handle other instance set by EssentialsLoader script
            {
                Destroy(gameObject); // destroys duplicate player created by scene change
            }
        }
        DontDestroyOnLoad(gameObject); // DontDestroyOnLoad prevents player from being destroyed on scene load
                                       // gameObject always refers to component this script is attached to, in this case the player
    }

    // Update is called once per frame
    void Update()
    {
		if(canMove) // checks if canMove is true
        {
            theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed; // assigns value to the velocity (built-in Rigidbody2D) of the player, using Input.GetAxisRaw function in Unity
                                                                                                                    // multiplies velocity vector by moveSpeed, allowing player move speed to be adjusted
        }
        else
        {
            theRB.velocity = Vector2.zero; // forces player Rigidbody2D velocity to zero if canMove is false
        }

        // assigns player Rigidbody2D velocity to "moveX" and "moveY" values in myAnim
        // allows the animator to use correct movement animations based on player movement direction
        myAnim.SetFloat("moveX", theRB.velocity.x);
		myAnim.SetFloat("moveY", theRB.velocity.y);

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1 || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1) // checks if player is moving in any direction - edited 10/12/19 to use absolute value
        {
            if (canMove) // checks if player can move to prevent unwanted idle animation changes
            {
                // assigns player x-y movement direction to "lastMoveX" and "lastMoveY" values in myAnim
                // allows the animator to use correct idle animation based on last player movement direction
                myAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
                myAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
            }
		}
        
        // clamps transform.position of object on this script, in this case the player
        // uses Mathf.Clamp function to perform the clamping
        // does not alter the z position
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight) // creates new SetBounds function to handle setting the bounds of the player in the map
                                                             // must pass two Vector3 values to handle bottom-left and top-right limits of map
    {
        bottomLeftLimit = botLeft + new Vector3(0.5f, 1f, 0); // sets bottom-left limit to the passed botLeft value, plus a small offset to compensate for player sprite size
        topRightLimit = topRight + new Vector3 (-0.5f, -1f, 0); // sets top-right limit to the passed topRight value, plus a small offset to compensate for player sprite size
    }
}