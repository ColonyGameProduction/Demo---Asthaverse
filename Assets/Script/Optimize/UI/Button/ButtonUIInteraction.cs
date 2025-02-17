
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonUIInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected GameObject _highlightContainer;
    [SerializeField] protected AudioManager _am;
    protected virtual void Awake() 
    {
        ToggleHighlight(false);
    }
    protected virtual void Start()
    {
        _am = AudioManager.Instance;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _am.PlayUIClick();
        ToggleHighlight(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ToggleHighlight(false);
    }

    public virtual void ToggleHighlight(bool change)
    {
        _highlightContainer.SetActive(change);
    }
}
