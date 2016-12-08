using UnityEngine;
using System.Collections;

// Part of another serlizable data storage class just like WeaponDefinition
[System.Serializable]
public class Part
{
    // These three fields need to be defined in the inspector pane 
    public string name; // The name of this part
    public float health; // The amount of health this part has 
    public string[] protectedBy; // The other pars that protect this 

    // These two fields are set automatically in Start()
    // Caching like this makes it faster and easier to find these later 
    public GameObject go;  // This GameObject of this part 
    public Material mat; // The Material to show damage 
}

public class Enemy_4 : Enemy
{
    // Enemy_4 will start Offscreeen and then pick a random point on screen to move to. 
    // Once it has arrived, it will pick another random point and continute until the player has shot it down

    public Vector3[] points; // Stores p0 & p1 for interpoilation 
    public float timeStart; // Birth time for this Enemy_4
    public float duration = 4; // Duration of movement 
    public Part[] parts; // The array of ship parts

    // Use this for initialization
    void Start()
    {
        points = new Vector3[2];
        // There is already an intial position chose by Main.Spawn Enemy()
        // So add it to points as this intial p0 & p1
        points[0] = pos;
        points[1] = pos;
        InitMovement();

        // Cache gameObject & material of each part in parts 
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }


    void InitMovement()
    {
        // pick a new point to move that is on screen 
        Vector3 p1 = Vector3.zero;
        float esp = Main.S.enemySpawnPadding;
        Bounds cBounds = Utils.camBounds;
        p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
        p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);

        points[0] = points[1]; // Shift points [1] to points [0]
        points[1] = p1; ; // Pass p1 as points [1]

        // Reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        // This completely overrides Enemy.Move() with linear interpolation
        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement();
            // Then initialize movement to a new point 
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // Apply ease out easing to u 
        pos = (1 - u) * points[0] + u * points[1]; // Simple linear interpolation 
    }

    // This will override the OnCollisionEnter that is part of Enemy.cs
    // Because of the way the Monobehaviour declares common Unity functions
    // Like OncollisionEnter(), the override keyword is not necessary 
    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // Enemies don't take damage unless they're on screen
                // This stop the player from shotting before they are visable
                bounds.center = transform.position + boundsCenterOffset;
                if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero)
                {
                    Destroy(other); // Destory the ProjectileHero
                    break;
                }

            // Hurt this enemy 
            // Find the gameObject that was hit
            /* 
             * The collision coll has contacts [], an array Contact points 
             * Because there was a collision, we're guaranteed there is at 
             * Least a contacts [0] and contact points have reference to 
             * his collider, which will be the collider for the part off the 
             * Enemy_5 that was hit 
            */

            GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    /* If prtHit wasnt found...
                    ...then it's usually because, very rarely, thisCollider on
                    contacts[0] will be the ProjectileHero instead of the ship
                    part. If so, just look for other Collider instead
                     */
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                // Check whether this part is still protected 
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        // If one of the protecting parts hasn't been destroyed....
                        if (!Destroyed(s))
                        {
                            // ...then don't damage this part yet 
                            Destroy(other); // Destoy the ProjectileHero
                            return; // Return before causing damage 
                        }
                    }
                }

                // It's not protected, so make it take damage
                // Get the damage amount from the Projectile.type & Main.W_DEFS
                prtHit.health -= Main.W_DEFS[p.type].damageOnHit;

                // Show damage on the part 
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    // Instead of Destroying this enemy, diable the damaged part 
                    prtHit.go.SetActive(false);
                }

                // Check to see if the whole ship is destroted 
                bool allDestroyed = true; // Assume it is destroyed 
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        // if a part still exists 
                        allDestroyed = false; // ...change allDestroyed to false
                        break;                // and break out of the foreach loop
                    }
                }
                // If it is completely destoyed 
                // Tell the main singleton that this ship has been destoyed 
                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    // Destoy this Enemy
                    Destroy(this.gameObject);
                }

                Destroy(other);
                break;
        }
    }

    // These two functions find a part in parts baes on name or GameObject 
    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return prt;
            }
        }
        return null;
    }
    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return prt;
            }
        }
        return null;
    }

    //These functions return true if the part has been destroyed 
    bool Destroyed(GameObject go)
    {
        return Destroyed(FindPart(go));
    }
    bool Destroyed(string n)
    {
        return Destroyed(FindPart(n));
    }

    bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            // If no real ph was passed in
            return (true); // return true (meaning, yes it was destroyed
        }
        // Return the result of rhe comparison: prt.health <=0
        // If prt.health is 0 or less, return true (yes, ut was destoyed)
        return (prt.health <= 0);
    }

    //This changes the color just one Part to red instead of the whole ship
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        remainingDamageFrames = showDamageForFrames;
    }
}