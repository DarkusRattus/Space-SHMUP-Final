using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class StartUI : MonoBehaviour
{
    public GameObject[] menuObjects;
    private GameObject currentButton;

    // Use this for initialization
    void Start()
    {
        currentButton = menuObjects[0]; // Assign the current button to be the first button on the pause screen
        EventSystem.current.SetSelectedGameObject(currentButton); // This is the selected button
    }

    // Update is called once per frame
    void Update()
    {
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

            EventSystem.current.SetSelectedGameObject(currentButton);  // Call the method that actually selects the button
        }
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