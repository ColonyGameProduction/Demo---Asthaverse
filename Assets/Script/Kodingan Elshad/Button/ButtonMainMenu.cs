using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonMainMenu : ButtonParent, IPointerEnterHandler, IPointerExitHandler
{
    public bool isGameOver;
    public GameObject highlightGameObject;

    public override void ButtonPressed()
    {
        base.ButtonPressed();
        SceneManager.LoadScene("");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isGameOver)
        {
            highlightGameObject.SetActive(true);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isGameOver)
        {
            highlightGameObject.SetActive(false);
        }
    }
}
