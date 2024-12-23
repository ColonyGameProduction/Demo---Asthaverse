
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonUIInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _highlightContainer;
    protected virtual void Awake() 
    {
        ToggleHighlight(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        ToggleHighlight(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ToggleHighlight(false);
    }

    public void ToggleHighlight(bool change)
    {
        _highlightContainer.SetActive(change);
    }
}
