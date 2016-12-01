using UnityEngine;
using System.Collections;

//Enemy_1 extends the Enemy class
public class Enemy_1 : Enemy {
    // Because Enemy_1 extends Enemy, the ___bool won't work
    // The same way in the Inspector pane. :/

    // # seconds for a full sine wave 
    public float        waveFrequencey = 2;
    // Since wave width in meters
    public float        waveWidth=4;
    public float        waveRotY = 45;

    private float       x0 = -12345; // the initial x values of pos
    private float       birthTime;

	// Use this for initialization
	void Start ()
    {
        // Set x0 to the initial x postion of Enemy_1
        // This works fine because the position will have already 
        // been set by Main.SpawnEnemy() before start() runs
        // (though Awake() would have been too early!).
        // This is also good because there is no Start() method
        // On enemy
        x0 = pos.x;

        birthTime =Time.time;
	}

    // Override the move function on enemy
    public override void Move()
    {
        // Because pos is a property, you can't directly set pos.x
        // So get the pos an editable Vector3
        Vector3 tempPos = pos;
        // theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequencey;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;

        // Rotate a bit about y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        // base.Move() still heandles the movement down in Y
        base.Move();
    }
}