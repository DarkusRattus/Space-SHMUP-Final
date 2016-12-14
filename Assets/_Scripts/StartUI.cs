using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class StartUI : MonoBehaviour
{
    public GameObject[] menuObjects;
    public GameObject[] optionsObjects;

    public bool playSounds = true;
    public bool playMusic = true;
    public bool playCopyrightSounds = true;

    private GameObject currentButton;
    private bool isOptions = false;

    // Use this for initialization
    void Start()
    {
        currentButton = menuObjects[0]; // Assign the current button to be the first button on the pause screen
        EventSystem.current.SetSelectedGameObject(currentButton); // This is the selected button

        // Set the toggles to match the PlayerPrefs
        optionsObjects[0].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Sound On"));
        optionsObjects[1].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Music On"));
        optionsObjects[2].GetComponent<Toggle>().isOn = GetBool(PlayerPrefs.GetInt("Copyright On"));

        if (playMusic) GetComponent<AudioSource>().Play();
        hideOptions();
    }

    // Update is called once per frame
    void Update()
    {
        if (playMusic && !GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
        else if (!playMusic && GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop();

        if (isOptions)
        {
            PlayerPrefs.SetInt("Sound On", GetNum(optionsObjects[0].GetComponent<Toggle>().isOn));
            PlayerPrefs.SetInt("Music On", GetNum(optionsObjects[1].GetComponent<Toggle>().isOn));
            PlayerPrefs.SetInt("Copyright On", GetNum(optionsObjects[2].GetComponent<Toggle>().isOn));
        }

        if(Input.GetKeyDown(KeyCode.Escape) && isOptions)
        {
            hideOptions();
        }

        // Navigate through the buttons
        // THIS CODE IS A MESS LIKE ME HAHAHAHA
        // Note: two buttons can be highlighted at the same time if you're using a mouse at the same time as your keyboard
        // I'm going to call this a feature
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            int currentPos; // Position of the currently selected button
            int pos = 0; // Temp variable for iteration
            bool downKey; // Are we working with the up key or down key?
            if (Input.GetKeyDown(KeyCode.DownArrow)) downKey = true;
            else downKey = false;
            if (!isOptions)
            {
                foreach (GameObject button in menuObjects)
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
                if (pos >= menuObjects.Length - 3 && downKey) currentPos = 0;
                else if (downKey) currentPos = pos + 1;
                else if (pos == 0) currentPos = menuObjects.Length - 3;
                else currentPos = pos - 1;
                currentButton = menuObjects[currentPos]; // Assign the current button
            }
            else
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

    // Show the UI elements on the Menu screen
    public void showMenu()
    {
        foreach (GameObject g in menuObjects)
        {
            g.SetActive(true);
        }
        currentButton = menuObjects[0]; // Assign the current button to be the first button on the pause screen
        EventSystem.current.SetSelectedGameObject(currentButton); // Set the current button
    }

    // Hide the Menu UI elements
    public void hideMenu()
    {
        foreach (GameObject g in menuObjects)
        {
            g.SetActive(false);
        }
        if (!isOptions) Time.timeScale = 1;
    }

    // Show the UI elements on the Options screen
    public void showOptions()
    {
        isOptions = true;
        hideMenu(); // Hide the Paused menu while the Options menu is active
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
        showMenu();
        isOptions = false;
    }

    // Sets the sound settings
    // For 'choice', 1 is Sound Effects, 2 is Backgound Music, 3 is Copyright Music
    public void ConfigureAudio(int choice)
    {
        if (choice == 1) playSounds = !playSounds;
        else if (choice == 2) playMusic = !playMusic;
        else if (choice == 3) playCopyrightSounds = !playCopyrightSounds;
    }

    public void PlayClassic()
    {
        SceneManager.LoadScene("_Classic_Mode");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}