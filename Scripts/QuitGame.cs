using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject button;
    public GameObject tutorial;

    bool activeYesOrNo = false;
    bool tutorialDone = false;

    private void Start()
    {
        if (tutorial != null)
        {
            tutorial.SetActive(true);
            Time.timeScale = 0;
        }

        if (tutorial == null)
            tutorialDone = true;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        tutorialDone = true;
    }


    public void LeaveGame()
    {
        Debug.Log("I've been clicked");
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && tutorialDone)
            if (!activeYesOrNo)
            {
                Cursor.visible = true;
                Time.timeScale = 0;
                button.SetActive(true);
                activeYesOrNo = !activeYesOrNo;
            }
            else
            {
                Cursor.visible = false;
                Time.timeScale = 1;
                button.SetActive(false);
                activeYesOrNo = !activeYesOrNo;
            }

    }
}
