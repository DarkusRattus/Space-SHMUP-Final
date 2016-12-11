using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

// Adapted from a SitePoint tutorial

public class UIManager : MonoBehaviour {

    public static UIManager S; // Singleton

    // We should access these through the Inspector pane. 
    // Because I like it that way.
    public int bonusScore = 200; // Amount of points the $ powerup gives you
    public Text scoreText; // Score text
    public Text addedScoreText; // Added score text
    public GameObject shieldBar; // Shield health image
    public GameObject invText; // The countdown when you grab an invulnerability PowerUp

    // These should be assigned in the Inspector pane 
    public GameObject[] pauseObjects; // Collection of options on the pause screen
    public GameObject[] finishObjects; // Collection of objects on the Game Over screen
    public GameObject[] helpObjects; // Collection of objects on the Help screen
    public GameObject[] optionsObjects; // Collection of objects on the Options screen
                                        
    private GameObject currentButton; // The highlighted button
    private Color originalAddedScoreColor; // The original color of the addedScore

    private bool isPaused = false; // Are we on the pause menu?
    private bool isOptions = false; // Are we on the options menu?
    private bool isHelp = false; // Are we on the help menu?
    private bool endGameTriggered = false; // Has the Game Over screen shown up yet?
    private float shieldLength; // Original shield vector length

    void Awake()
    {
        S = this;
    }

    // Use this for initialization
    void Start()
    {
        // Initialize some things
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        addedScoreText = GameObject.Find("AddedPoints").GetComponent<Text>();
        shieldBar = GameObject.Find("HealthMeter");
        invText = GameObject.Find("InvText");


        shieldLength = shieldBar.transform.localScale.x; // Get the length of the shield bar
        originalAddedScoreColor = addedScoreText.GetComponent<Text>().color; // Grab the original score of AddedScore
        invText.SetActive(false); // The Hero is not invulnerable as of the beginning of this countdown
        Time.timeScale = 1; // Game is running at normal time (not paused)
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
            showFinished(); // Show the End Game buttons
            if (!endGameTriggered)
            {
                currentButton = finishObjects[0]; // Assign the current button to the first button on the Game Over screen
                EventSystem.current.SetSelectedGameObject(currentButton); // Set the currett button
                endGameTriggered = true; // The game has ended
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
            bool downKey = (Input.GetKeyDown(KeyCode.DownArrow)); // Are we pressing the down key?

            //if (Input.GetKeyDown(KeyCode.DownArrow)) downKey = true;
           // else downKey = false;
           
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

        if (Hero.S.shieldLevel < 100)
        {
            shieldBar.transform.localScale = new Vector3(shieldLength * (Hero.S.shieldLevel / Hero.S.maxShieldLevel),
                                                        shieldBar.transform.localScale.y,
                                                        shieldBar.transform.localScale.z);
        }
        else
        {
            shieldBar.transform.localScale = new Vector3(shieldLength, shieldBar.transform.localScale.y, shieldBar.transform.localScale.z);
        }
        scoreText.text = "Score: " + Hero.S.score;

        // Display the invincible text if the Hero is invincible
        if (Hero.S.invincible)
        {
            invText.SetActive(true);
            shieldBar.SetActive(false);
        }
        else
        {
            invText.SetActive(false);
            shieldBar.SetActive(true);
        }
    }



    // Restarts the level
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //

    // Update the score
    public void AddScore(int points)
    {
        Hero.S.score += points;
        addedScoreText.GetComponent<Text>().text = "+" + points;
        StartCoroutine(addUpdate(2.5f));
    }

    private IEnumerator addUpdate(float seconds)
    {
        addedScoreText.GetComponent<Text>().color = originalAddedScoreColor; // Reset the color / alpha
        float alpha = addedScoreText.GetComponent<Text>().color.a; // Grab that alpha
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / seconds)
        {
            Color newColor = new Color(220, 255, 0, Mathf.Lerp(alpha, 0, t));
            addedScoreText.GetComponent<Text>().color = newColor;
            yield return null;
        }
        yield return new WaitForSeconds(seconds);
        addedScoreText.GetComponent<Text>().text = "";
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
        shieldBar.SetActive(false);
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
