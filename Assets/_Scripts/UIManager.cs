using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

// Adapted from a SitePoint tutorial

public class UIManager : MonoBehaviour {

    public static UIManager S; // Singleton
    public static int highScore; // High score pulled from PlayerPrefs

    // We should access these through the Inspector pane. 
    // Because I like it that way.
    public int bonusScore = 200; // Amount of points the $ powerup gives you
    public Text scoreText; // Score text
    public Text addedScoreText; // Added score text
    public Text finishScore;
    public Text finishHighScore;
    public GameObject shieldBar; // Shield health image
    public GameObject invText; // The countdown when you grab an invulnerability PowerUp

    // These should be assigned in the Inspector pane 
    public GameObject[] pauseObjects; // Collection of options on the pause screen
    public GameObject[] finishObjects; // Collection of objects on the Game Over screen
    public GameObject[] optionsObjects; // Collection of objects on the Options screen
    
    private GameObject currentButton; // The highlighted button
    private Color originalAddedScoreColor; // The original color of the addedScore

    private bool isPaused = false; // Are we on the pause menu?
    private bool isOptions = false; // Are we on the options menu?
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
        highScore = PlayerPrefs.GetInt("High Score");

        scoreText = GameObject.Find("Score").GetComponent<Text>();
        addedScoreText = GameObject.Find("AddedPoints").GetComponent<Text>();
        shieldBar = GameObject.Find("HealthMeter");
        invText = GameObject.Find("InvText");

        // Set the toggles to match the PlayerPrefs
        optionsObjects[0].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Sound On"));
        optionsObjects[1].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Music On"));
        optionsObjects[2].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Copyright On"));

        shieldLength = shieldBar.transform.localScale.x; // Get the length of the shield bar
        originalAddedScoreColor = addedScoreText.GetComponent<Text>().color; // Grab the original score of AddedScore
        invText.SetActive(false); // The Hero is not invulnerable as of the beginning of this countdown
        Time.timeScale = 1; // Game is running at normal time (not paused)
        hideOptions(); // This will put the Paused menu up, but we'll hide it immediately after
        hidePaused();
        hideFinished();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOptions)
        {
            PlayerPrefs.SetInt("Sound On", GetNum(optionsObjects[0].GetComponent<Toggle>().isOn));
            PlayerPrefs.SetInt("Music On", GetNum(optionsObjects[1].GetComponent<Toggle>().isOn));
            PlayerPrefs.SetInt("Copyright On", GetNum(optionsObjects[2].GetComponent<Toggle>().isOn));
        }

        MenuNav();

        // Player is kill (RIP in piece)
        if (Time.timeScale == 0 && Hero.S.shieldLevel < 0 && !endGameTriggered)
        { 
            showFinished();
        }

        // Handle the ShieldBar
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

        // Show the score
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

    // Show the UI elements on the Pause screen
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
        currentButton = pauseObjects[0]; // Assign the current button to be the first button on the pause screen
        EventSystem.current.SetSelectedGameObject(currentButton); // Set the current button
    }

    // Hide the Paused UI elements
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
        if(!isOptions) Time.timeScale = 1;
    }

    // Show the UI elements on the Options screen
    public void showOptions()
    {
        isOptions = true;
        hidePaused(); // Hide the Paused menu while the Options menu is active
        currentButton = optionsObjects[0]; // Pre-select the first object
        currentButton.GetComponentInChildren<Text>().color = Color.yellow; // Let the player know the option is selected
        foreach (GameObject g in optionsObjects)
        {
            g.SetActive(true);
        }
        optionsObjects[0].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Sound On"));
        optionsObjects[1].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Music On"));
        optionsObjects[2].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Copyright On"));
        EventSystem.current.SetSelectedGameObject(currentButton); // Set the current button
    }

    // Hide the Options UI elements
    public void hideOptions()
    {
        foreach (GameObject g in optionsObjects)
        {
            g.SetActive(false);
        }
        showPaused();
        isOptions = false;
    }

    // Shows the End Game objects
    public void showFinished()
    {
        endGameTriggered = true;
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(true);
        }
        if (Hero.S.score > highScore)
        {
            PlayerPrefs.SetInt("High Score", Hero.S.score);
            finishHighScore.GetComponent<Text>().text = "Old High Score: " + highScore;
            finishScore.GetComponent<Text>().text = "NEW HIGH SCORE: " + Hero.S.score;
        }
        else if (Hero.S.score == highScore)
        {
            finishHighScore.GetComponent<Text>().text = "You matched the current High Score";
            finishScore.GetComponent<Text>().text = "High Score: " + highScore;
        }
        else
        {
            finishHighScore.GetComponent<Text>().text = "High Score: " + highScore;
            finishScore.GetComponent<Text>().text = "Your Score: " + Hero.S.score;
        }
        scoreText.GetComponent<Text>().text = "";
        shieldBar.SetActive(false);
        currentButton = finishObjects[0]; // Assign the current button to the first button on the Game Over screen
        EventSystem.current.SetSelectedGameObject(currentButton); // Set the current button
    }

    // Hides the End Game objects
    public void hideFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(false);
        }
    }


    // Handles all menu navigation
    public void MenuNav()
    { 
        // Pause dat shit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1 && Hero.S.shieldLevel >= 0)
            {
                Time.timeScale = 0; // STOP TIME
                showPaused(); // Show the buttons

                EventSystem.current.SetSelectedGameObject(currentButton); // This is the selected button

                isPaused = true; // We iz paused
            }
            else if (Time.timeScale == 0 && Hero.S.shieldLevel >= 0 && !endGameTriggered)
            {
                if (isOptions) // Are we in the Options menu?
                {
                    hideOptions(); // Hide the Options buttons
                }
                else
                {
                    Time.timeScale = 1; // Start time again
                    hidePaused(); // Hide the buttons
                    isPaused = false; // No longer paused
                    isOptions = false; // Just in case
                }
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

            if (isPaused && !isOptions) // Are we paused?
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
            else if (isOptions) // We're in the Options menu
            {
                foreach (GameObject button in optionsObjects)
                {
                    if (currentButton == button)
                    {
                        currentPos = pos; // Assign the button to the position
                        break; // We found the button, get outta here
                    }
                    pos++;
                }
                // Length-3 instead of Length-2, cuz we have the Quad and the UI Text
                if (pos >= optionsObjects.Length - 3 && downKey) currentPos = 0;
                else if (downKey) currentPos = pos + 1;
                else if (pos == 0) currentPos = optionsObjects.Length - 3;
                else currentPos = pos - 1;
                currentButton = optionsObjects[currentPos]; // Assign the current button
                // Let us let the player know which non-button option is selected
                if (currentButton.GetComponent<Toggle>() != null)
                {
                    currentButton.GetComponentInChildren<Text>().color = Color.yellow;
                }
                foreach (GameObject button in optionsObjects)
                {
                    if (button != currentButton && button.GetComponent<Toggle>() != null)
                    {
                        button.GetComponentInChildren<Text>().color = Color.white;
                    }
                }
                optionsObjects[0].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Sound On"));
                optionsObjects[1].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Music On"));
                optionsObjects[2].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Copyright On"));
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
                if (pos >= finishObjects.Length - 5 && downKey) currentPos = 0;
                else if (downKey) currentPos = pos + 1;
                else if (pos == 0) currentPos = finishObjects.Length - 5;
                else currentPos = pos - 1;

                currentButton = finishObjects[currentPos]; // Assign the current button

            }
            EventSystem.current.SetSelectedGameObject(currentButton);  // Call the method that actually selects the button

        }

    }

    public bool GetBool(int num)
    {
        return (num == 1);
    }

    public int GetNum(bool val)
    {
        if (val) return (1);
        else return (0);
    }

    // Sets the sound settings
    // For 'choice', 1 is Sound Effects, 2 is Backgound Music, 3 is Copyright Music
    public void ConfigureAudio(int choice)
    {
        if (choice == 1) AudioManager.S.playSounds = !AudioManager.S.playSounds;
        else if (choice == 2) AudioManager.S.playMusic = !AudioManager.S.playMusic;
        else if (choice == 3) AudioManager.S.playCopyrightSounds = !AudioManager.S.playCopyrightSounds;
    }

}
