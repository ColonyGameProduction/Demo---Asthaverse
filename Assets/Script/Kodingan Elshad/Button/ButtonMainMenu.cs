using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
