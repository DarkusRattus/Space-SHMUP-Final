using UnityEngine;
using System.Collections;

public class FloatingScore : MonoBehaviour {

    public int timeDuration = 5; // Length of time to keep the text on screen

    // Use this for initialization
    void Start ()
    {
        destroyMe(timeDuration);
    }
	
    // We want to remove this GUI Text when the time runs out
    IEnumerator destroyMe(int seconds)
    {
        yield return new WaitForSeconds(timeDuration);
        Destroy(this.gameObject);
    }

}
