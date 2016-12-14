using UnityEngine;
using System.Collections;
 
public class Hero : MonoBehaviour {

	static public Hero S; // Singleton

    public float gameRestartDelay = 2f;

	// These fields control the movement of the ship
	public float speed = 30;
    public float score = 0;
    public float maxShieldLevel = 4;
	public float rollMult = -45;
	public float pitchMult = 30;

    public bool invincible = false; // for the invincibility PowerUp
    public float invulnerableTime = 15f; // invincibility lasts for x seconds
    public GameObject invParticles; // Particle effect for invulnerability 


	// Ship status information
    [SerializeField]
	private float _shieldLevel = 1;

    // Weapon fields
    public Weapon[] weapons;

	public bool ________________;

	public Bounds bounds;

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Create a WeaponFileDelegate field named fireDelegate
    public WeaponFireDelegate fireDelegate;

    private bool audioStopped = false;

	void Awake(){
		S = this; // Set the singleton
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
	}
	
    // Add Start() to resolve race condition
    void Start()
    {
        // Reset the weapons to start _Hero with 1 blaster
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);

        invParticles = GameObject.Find("InvParticles");
        invParticles.SetActive(false); // You are not invincible at the game start
    }

	// Update is called once per frame
	void Update () {

		// Pull in infomation from the Input class
		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		// Change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		bounds.center = transform.position;

		// Keep te ship constrained to the screen bounds
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.onScreen);
		if (off != Vector3.zero) {
			pos -= off;
			transform.position = pos;
		}

		// Rotate the ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler (yAxis*pitchMult, xAxis*rollMult, 0);

        // Use the fireDelegate to fire Weapons
        // First, make sure the Axis("Jump") button is pressed
        // Then ensure that fireDelegate isn't null to avoid an error
        if(Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }

        if (!AudioManager.S.playCopyrightSounds && GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();
        else if (AudioManager.S.playCopyrightSounds && invincible && !GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Play();
    }

    // This variable holds a reference to the last triggering GameObject
    public GameObject lastTriggerGo = null;

    void OnTriggerEnter(Collider other)
    {
        // Find tag of other.gameObjet or its parent GameObjects
        GameObject go = Utils.FindTaggedParent(other.gameObject);
        // If there is a parent with a tag
        if(go != null) {
            // Make sure it's not the same triggering go as last time
            if (go == lastTriggerGo)
            {
                return;
            }
            lastTriggerGo = go;

            if (go.tag == "Enemy")
            {
                // If the Shield was triggered by an enemy
                // Decrease the level of the shield by 1
                if (!invincible) shieldLevel--;
                if (invincible) UIManager.S.AddScore(go.GetComponent<Enemy>().score);
                // Destroy the enemy
                Destroy(go);
            }
            else if(go.tag == "PowerUp")
            {
                // If the shield was triggered by a PowerUp
                AbsorbPowerUp(go);
            }
            else
            {
                print("Triggered: " + go.name);
            }

        }
        else
        {
            // Otherwise, announce the original other.gameObject
            print("Triggered: " + other.gameObject.name);
        }


    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        if (AudioManager.S.playSounds)
        {
            PowerUp.S.GetComponent<AudioSource>().Play();
        }
        switch (pu.type)
        {
            case WeaponType.shield: // If it's the Shield
                shieldLevel++;
                break;

            case WeaponType.gold: // If it's GOLD
                UIManager.S.AddScore(UIManager.S.bonusScore);
                break;

            case WeaponType.invincibility: // If it's the invincibility one
                if (AudioManager.S.playCopyrightSounds)
                {
                    AudioManager.S.GetComponent<AudioSource>().Stop();
                    if (!GetComponent<AudioSource>().isPlaying)
                        GetComponent<AudioSource>().Play();
                }
                else
                    GetComponent<AudioSource>().Stop();
                StartCoroutine(invincibilityCountdown());
                break;

            default: // If it's any Weapon PowerUp
                // Check the current Weapon type
                if (pu.type == weapons[0].type)
                {
                    // Then increase the number of Weapons of this type
                    Weapon w = GetEmptyWeaponSlot(); // Find an available Weapon
                    if (w != null)
                    {
                        // Set it to pu.type
                        w.SetType(pu.type);
                    }
                }
                else
                {
                    // If this is a different Weapon
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
                
        }
        pu.AbsorbedBy(this.gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

    // Invincibility countdown
    private IEnumerator invincibilityCountdown()
    {
        invincible = true; 
        invParticles.SetActive(true); // Show the invincibility particles

        yield return new WaitForSeconds(invulnerableTime); // Wait for the invincibility time to run out

        if (AudioManager.S.playCopyrightSounds)
        {
            GetComponent<AudioSource>().Stop();
            if(AudioManager.S.playMusic)
                AudioManager.S.GetComponent<AudioSource>().Play();
        }
        invincible = false;
        invParticles.SetActive(false); // Hide the invincibility particles
    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, maxShieldLevel);
            // If the is going to be set to less than zero
            if (value < 0)
            {
                Destroy(this.gameObject);
                Time.timeScale = 0;
            }
        }
    }
}
