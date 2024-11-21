using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private float highlightedAlpha = 1f;
    [SerializeField] private float normalAlpha = 0.1f;

    private void Start()
    {
        if (highlight != null)
        {
            highlight.SetActive(false);
        }

        if (buttonText != null)
        {
            SetTextAlpha(normalAlpha);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlight != null)
        {
            highlight.SetActive(true);
        }

        if (buttonText != null)
        {
            SetTextAlpha(highlightedAlpha);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlight != null)
        {
            highlight.SetActive(false);
        }

        if (buttonText != null)
        {
            SetTextAlpha(normalAlpha);
        }
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = buttonText.color;
        color.a = alpha;
        buttonText.color = color;
    }
}