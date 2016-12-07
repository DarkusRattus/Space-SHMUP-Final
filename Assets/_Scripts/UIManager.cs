using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

// Adapted from a SitePoint tutorial

public class UIManager : MonoBehaviour {

    public GameObject firstButtonPause;
    public GameObject firstButtonGameOver;

    [SerializeField]
    private GameObject[] pauseObjects;
    [SerializeField]
    private GameObject[] finishObjects;

    private GameObject currentButton;

    private bool isPaused = false;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
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

                currentButton = firstButtonPause;

                EventSystem.current.SetSelectedGameObject(firstButtonPause); // This is the selected button

                // CHANGE THE HIGHLIGHTED BUTTON COLOR
                /*Button b = firstButtonPause.GetComponent<Button>();
                ColorBlock cb = b.colors;
                cb.highlightedColor = Color.red;
                b.colors = cb; */

                SelectButton(currentButton, false);

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
        }

        // Navigate through the buttons
        if (Time.timeScale == 0 && Input.GetKeyDown(KeyCode.DownArrow))
        {
            int currentPos; // Position of the currently selected button
            int pos = 0;
            if (isPaused)
            { 
                foreach (GameObject button in pauseObjects)
                {
                    if (currentButton == button)
                    {
                        currentPos = pos;
                        break;
                    }
                    pos++;
                }
                if(pos >= pauseObjects.Length - 2)
                {
                    currentPos = 0;
                }
                else
                {
                    currentPos = pos+1;
                }
                currentButton = pauseObjects[currentPos];
            }
            else
            {
                foreach (GameObject button in finishObjects)
                {
                    if (currentButton == button)
                    {
                        currentPos = pos;
                        break;
                    }
                    pos++;
                }
                if (pos >= finishObjects.Length - 2)
                {
                    currentPos = 0;
                }
                else
                {
                    currentPos = pos+1;
                }
                currentButton = finishObjects[currentPos];
            }
            SelectButton(currentButton, true);
        }
    }

    // Select a button
    public void SelectButton(GameObject selectedButton, bool setActive)
    {
        EventSystem.current.SetSelectedGameObject(selectedButton);
        Button b = selectedButton.GetComponent<Button>();
        ColorBlock cb = b.colors;
        cb.highlightedColor = Color.red;
        b.colors = cb;
        selectedButton.GetComponent<Button>().colors = cb;

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
