// AttackEffect
// handles the display of attack effects in battle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public float effectLength; // creates float to handle length of attack effect
    public int soundEffect; // creates int to to handle playback of attack sound

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlaySFX(soundEffect); // calls audio manager to play back associated sound effect
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, effectLength); // destroys attack effect object after length of time is complete
    }
}
