using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Adapted from a SitePoint tutorial

public class UIManager : MonoBehaviour {

    GameObject[] pauseObjects;
    GameObject[] finishObjects;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;

        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");             // Paused UI elements
        finishObjects = GameObject.FindGameObjectsWithTag("ShowOnEndGame");          // End Game UI elements

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
                Time.timeScale = 0;
                showPaused();
            }
            else if (Time.timeScale == 0 && Hero.S.shieldLevel >= 0)
            {
                Time.timeScale = 1;
                hidePaused();
            }
        }

        // Player is kill (RIP in piece)
        if (Time.timeScale == 0 && Hero.S.shieldLevel < 0)
        {
            showFinished();
        }
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
            Time.timeScale = 1;
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
