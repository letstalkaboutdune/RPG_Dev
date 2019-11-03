// CameraController
// handles the position and bounds of camera around player and map

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// includes Unity Tilemap library
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform target; // creates Transform variable to handle position of target, in this case the camera

    public Tilemap theMap; // creates reference to the tilemap to handle map bounds

    private Vector3 bottomLeftLimit, topRightLimit; // creates Vector3 variables to handle bottom-left and top-right limits of map
                                                    // only these two limits are needed to fully bound camera in 2D space
    private float halfHeight, halfWidth; // creates float variables to handle the half-height and half-width of camera size
                                         // this adds an offset to camera position to make sure camera is always within bounds of scene
    // creates variables to manage music playback
    public int musicToPlay;
    private bool musicStarted;
    
        
    // Start is called before the first frame update
    void Start()
    {   
        target = FindObjectOfType<PlayerController>().transform; // uses FindObjectOfType function to find instance of PlayerController
                                                                     // sets camera transform target to the transform of the player        
        halfHeight = Camera.main.orthographicSize; // uses the Camera.main.orthographicSize function to pull the half-height of the camera

        halfWidth = halfHeight * Camera.main.aspect; // calculates the half-width using the half-height and the camera aspect ratio

        bottomLeftLimit = theMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f); // sets the bottom-left limit to the min of the localBounds of the linked tilemap
                                                                                            // adds an offset for half-width, half-height to compensate for camera size
        topRightLimit = theMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f); // sets the top-right limit to the max of the localBounds of the linked tilemap
                                                                                            // subtracts an offset for half-width, half-height to compensate for camera size
        PlayerController.instance.SetBounds(theMap.localBounds.min, theMap.localBounds.max); // calls the SetBounds function in the PlayerController instance in order to clamp player position in the map
                                                                                                // passes the min/max of the tilemap localBounds without the offset for camera size
    }


    void LateUpdate()  // LateUpdate is called once per frame after Update, prevents stuttering in camera motion
    {
        if (GameObject.Find("Player(Clone)") != null) // checks if instance of player controller exists
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z); // sets the (x, y) transform position of this camera controller to the position of the target/player
                                                                                                          // leaves the z transform position of the camera at its current value, since it needs to stay slightly zoomed out
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z); // uses the built-in Mathf.Clamp function to clamp the (x, y, z) transform position to the bottom-left and top-right limits
                                                                                                                                                                                                                  // forces the camera to stay within the appropriate position relative to the tilemap
        }

        if (!musicStarted) // checks if music is already started
        {
            musicStarted = true; // sets music started flag to true
            AudioManager.instance.PlayBGM(musicToPlay); // calls audio manager function to play music
        }
    }
}