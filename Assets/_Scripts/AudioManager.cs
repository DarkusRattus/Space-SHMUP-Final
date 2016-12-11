using UnityEngine;
using System.Collections;

// Used to handle the audio stuff
// Trying to do the weapon sounds within the Weapons file is pretty confusing

public class AudioManager : MonoBehaviour {

    public static AudioManager S; // Singleton

    public AudioClip backgroundMusic; // The music playing during the game
    public AudioClip invMusic; // Sanic
    public AudioClip soundBlaster; // The Blaster weapon sound
    public AudioClip soundSpread; // The Spread weapon sound
    public AudioClip soundExplosion; // The sound when an Enemy is destroyed

    public bool playMusic = true; // Enable playing background music
    public bool playSounds = true; // Enable sound effects
    public bool playCopyrightSounds = true; // Shh


    void Awake()
    {
        S = this;
    }
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
