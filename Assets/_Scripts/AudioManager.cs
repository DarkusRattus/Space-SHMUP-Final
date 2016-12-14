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

    private bool audioStopped = false;

    public AudioSource backgroundMusicS;

    void Awake()
    {
        S = this;
    }
    
	// Use this for initialization
	void Start ()
    {
        backgroundMusicS = new AudioSource();
        GetComponent<AudioSource>().clip = backgroundMusic;
        GetComponent<AudioSource>().loop = true;

        if (playMusic) GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {

        playSounds = UIManager.S.GetBool(PlayerPrefs.GetInt("Sound On"));
        playMusic = UIManager.S.GetBool(PlayerPrefs.GetInt("Music On"));
        playCopyrightSounds = UIManager.S.GetBool(PlayerPrefs.GetInt("Copyright On"));

        if (!playMusic)
        {
            GetComponent<AudioSource>().Stop();
        }
        else if (playMusic && !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
	}
}
