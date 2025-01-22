using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private Image tutorialTextImage;
    [SerializeField] private float textDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine timerCoroutine;

    private void OnEnable()
    {
        SetTextAlpha(0);

        LeanTween.value(gameObject, 0, 1, fadeDuration)
            .setOnUpdate(SetTextAlpha)
            .setOnComplete(() =>
            {
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);
                timerCoroutine = StartCoroutine(TextTimer());
            });
    }

    private void OnDisable()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        LeanTween.cancel(gameObject);
    }

    private IEnumerator TextTimer()
    {
        yield return new WaitForSeconds(textDuration);

        LeanTween.value(gameObject, 1, 0, fadeDuration)
            .setOnUpdate(SetTextAlpha)
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void SetTextAlpha(float alpha)
    {
        if (tutorialTextImage != null)
        {
            Color color = tutorialTextImage.color;
            color.a = alpha;
            tutorialTextImage.color = color;
        }
    }
}
