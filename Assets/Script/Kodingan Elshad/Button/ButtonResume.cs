using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonResume : ButtonParent, IPointerEnterHandler, IPointerExitHandler
{
    GameManager gm;
    public GameObject pauseCanvas;
    public GameObject highlightGameObject;

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
        highlightGameObject.SetActive(false);
        pauseCanvas.SetActive(false);
        gm.gameIsPaused = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    { 
        highlightGameObject.SetActive(true);        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightGameObject.SetActive(false);
    }

}
