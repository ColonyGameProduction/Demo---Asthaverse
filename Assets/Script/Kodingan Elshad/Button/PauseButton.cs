using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButton : ButtonParent
{
    GameManager gm;
    public GameObject pauseCanvas;
    private void Start()
    {
        gm = GameManager.instance;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonPause();
        }
        
    }

    public void ButtonPause()
    {
        if (gm.gameIsPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseCanvas.SetActive(false);
            Time.timeScale = 1f;
            gm.gameIsPaused = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseCanvas.SetActive(true);
            Time.timeScale = 0f;
            gm.gameIsPaused = true;
        }
    }
}
