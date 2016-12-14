using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    public static Shield S; // Singleton

	public float rotationsPerSecond = 0.1f;
	public bool ____________________;
	public int levelShown = 0;


	void Update () {
	
		// Read the current shield level from the Hero Singleton
		int currLevel = Mathf.FloorToInt (Hero.S.shieldLevel);
		// If this is different from levelShown...
		if (levelShown != currLevel) {
            if (currLevel >= 4) levelShown = 4;
            else levelShown = currLevel;
			Material mat = this.GetComponent<Renderer>().material;
			// Adjust the texture offset to show different shield level
			mat.mainTextureOffset = new Vector2 (0.2f * levelShown, 0);
		}
        if (Hero.S.invincible)
        {
            this.GetComponent<Renderer>().material.color = new Color(255, 0, 255, 0);
        }
        else if(currLevel / Hero.S.maxShieldLevel >= .75)
        {
            this.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (currLevel / Hero.S.maxShieldLevel >= .40)
        {
            this.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
        float rZ = (rotationsPerSecond * Time.time * 360) % 360f;
		transform.rotation = Quaternion.Euler (0, 0, rZ);

	}
}
