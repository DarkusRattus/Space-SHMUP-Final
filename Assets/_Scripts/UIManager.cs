using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

// Adapted from a SitePoint tutorial

public class UIManager : MonoBehaviour {


    // We should access these through the inspector pane. 
    // Because I like it that way.
    public GameObject shieldBar; // Shield health image
    public GameObject[] pauseObjects; // Collection of options on the pause screen
    public GameObject[] finishObjects; // Collection of objects on the Game Over screen
                                        
    private GameObject currentButton; // The highlighted button

    private bool isPaused; // Are we on the pause menu?
    private bool endGameTriggered; // Has the Game Over screen shown up yet?
    private float shieldLength; // Original shield vector length

    // Use this for initialization
    void Start()
    {
        shieldLength = shieldBar.transform.localScale.x;
        Time.timeScale = 1;
        isPaused = false;
        endGameTriggered = false;
        hidePaused();
        hideFinished();
    }

    // Update is called once per frame
    void Update()
    {
        // Pause dat shit
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            if (Time.timeScale == 1 && Hero.S.shieldLevel >= 0)
            {
                Time.timeScale = 0; // STOP TIME
                showPaused(); // Show the buttons

                currentButton = pauseObjects[0]; // Assign the current button to be the first button on the pause screen
                EventSystem.current.SetSelectedGameObject(currentButton); // This is the selected button

                isPaused = true; // We iz paused
            }
            else if (Time.timeScale == 0 && Hero.S.shieldLevel >= 0)
            {
                Time.timeScale = 1; // Start time again
                hidePaused(); // Hide the buttons
                isPaused = false; // No longer paused
            }
        }
        
        // Player is kill (RIP in piece)
        if (Time.timeScale == 0 && Hero.S.shieldLevel < 0)
        {
            showFinished();
            if (!endGameTriggered)
            {
                currentButton = finishObjects[0]; // Assign the current button to the first button on the Game Over screen
                EventSystem.current.SetSelectedGameObject(currentButton);
                endGameTriggered = true; 
            }

        }

        // Navigate through the buttons
        // THIS CODE IS A MESS LIKE ME HAHAHAHA
        // Note: two buttons can be highlighted at the same time if you're using a mouse at the same time as your keyboard
        // I'm going to call this a feature
        if (Time.timeScale == 0 && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            int currentPos; // Position of the currently selected button
            int pos = 0; // Temp variable for iteration
            bool downKey; // Are we working with the up key or down key?
            if (Input.GetKeyDown(KeyCode.DownArrow)) downKey = true;
            else downKey = false;
           
            if (isPaused) // Are we paused?
            { 
                foreach (GameObject button in pauseObjects)
                {
                    if (currentButton == button)
                    {
                        currentPos = pos; // Assign the button to the position
                        break; // We found the button, get outta here
                    }
                    pos++;
                }
                // The last object in pauseObjects is going to be UI Text for the PAUSED label
                // Because I'm lazy...so putting at Length-2 skips it
                // If we're at Length-2, we're on the last button. So we're going to start at the first.
                if (pos >= pauseObjects.Length - 2 && downKey) currentPos = 0;
                else if (downKey) currentPos = pos + 1;
                else if (pos == 0) currentPos = pauseObjects.Length - 2;
                else currentPos = pos - 1;
                currentButton = pauseObjects[currentPos]; // Assign the current button
            }
            else // We're not paused...so we're on the Game Over screen.
            {
                // Iterate through the objects on the Game Over screen
                foreach (GameObject button in finishObjects)
                {
                    if (currentButton == button)
                    {
                        currentPos = pos;
                        break;
                    }
                    pos++;
                }
                // Same thing as before
                if (pos >= finishObjects.Length - 2 && downKey) currentPos = 0;
                else if (downKey) currentPos = pos + 1;
                else if (pos == 0) currentPos = finishObjects.Length - 2;
                else currentPos = pos - 1;
                
                currentButton = finishObjects[currentPos]; // Assign the current button
            }
            EventSystem.current.SetSelectedGameObject(currentButton);  // Call the method that actually selects the button
        }
        shieldBar.transform.localScale = new Vector3(shieldLength * (Hero.S.shieldLevel / Hero.S.maxShieldLevel),
                                                        shieldBar.transform.localScale.y,
                                                        shieldBar.transform.localScale.z);

    }



    // Restarts the level
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Pause control
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            hidePaused();
        }
    }

    // Show the UI elements on the Pause screen
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    // Hide the Paused UI elements
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
        Time.timeScale = 1;
    }

    // Shows the End Game objects
    public void showFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(true);
        }
    }

    // Hides the End Game objects
    public void hideFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(false);
        }
    }

}
