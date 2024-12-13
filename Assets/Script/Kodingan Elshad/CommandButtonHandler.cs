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
            currentIndex = 0;
            isPressed = true;
            SelectButton(currentIndex);
            commandButton[currentIndex].gameObject.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isPressed = false;
            if(EventSystem.current.currentSelectedGameObject == commandButton[currentIndex].gameObject)
            {
                commandButton[currentIndex].onClick.Invoke();
            }
            EventSystem.current.SetSelectedGameObject(null);
            commandButton[currentIndex].gameObject.SetActive(false);
        }

        if (isPressed)
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
        commandButton[currentIndex].gameObject.SetActive(false);
        currentIndex = (currentIndex + 1) % commandButton.Count;
        SelectButton(currentIndex);
        commandButton[currentIndex].gameObject.SetActive(true);
    }

    public void PrevButton()
    {
        commandButton[currentIndex].gameObject.SetActive(false);
        currentIndex = (currentIndex - 1 + commandButton.Count) % commandButton.Count;
        SelectButton(currentIndex);
        commandButton[currentIndex].gameObject.SetActive(true);
    }

    public void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(commandButton[index].gameObject);
    }

}
