using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [TextArea][SerializeField] private string fullText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float textDuration = 3f;

    private Coroutine typingCoroutine;

    private void Start()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeTextEffect());
    }

    private IEnumerator TypeTextEffect()
    {
        textComponent.text = "";
        foreach (char c in fullText.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(textDuration);

        textComponent.text = "";
    }
}
