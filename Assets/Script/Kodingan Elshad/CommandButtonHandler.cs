using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandButtonHandler : MonoBehaviour
{

    public List<Button> commandButton = new List<Button>();

    private int currentIndex = 0;
    private bool isPressed = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isPressed = true;
            SelectButton(currentIndex);
        }

        if(Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isPressed = false;
            if(EventSystem.current.currentSelectedGameObject == commandButton[currentIndex].gameObject)
            {
                commandButton[currentIndex].onClick.Invoke();
            }
            EventSystem.current.SetSelectedGameObject(null);
        }

        if(isPressed)
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

            if(scrollDelta > 0)
            {
                NextButton();
            }
            else if(scrollDelta < 0)
            {
                PrevButton();
            }
        }

    }

    public void NextButton()
    {
        currentIndex = (currentIndex + 1) % commandButton.Count;
        SelectButton(currentIndex);
    }

    public void PrevButton()
    {
        currentIndex = (currentIndex - 1 + commandButton.Count) % commandButton.Count;
        SelectButton(currentIndex);
    }

    public void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(commandButton[index].gameObject);
    }

}
