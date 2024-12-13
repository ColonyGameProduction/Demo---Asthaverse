using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonResume : ButtonParent
{
    GameManager gm;
    public GameObject pauseCanvas;

    private void OnEnable()
    {
        gm = GameManager.instance;
    }

    public void ReturnButton()
    {
        base.ButtonPressed();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        gm.gameIsPaused = false;
    }
}
