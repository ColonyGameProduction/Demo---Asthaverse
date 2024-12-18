
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonUIInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _highlightContainer;
    protected virtual void Awake() 
    {
        _highlightContainer.SetActive(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _highlightContainer.SetActive(true);  
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _highlightContainer.SetActive(false);
    }
}
