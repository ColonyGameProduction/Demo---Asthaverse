using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private float timeRemaining = 60f;
    [SerializeField] private TextMeshProUGUI timerText;
    private bool timerIsRunning = true;

    [Header("Effect")]
    private RectTransform _timerRect;
    [SerializeField] private Color _warningTimer1, _warningTimer2;
    [SerializeField] private float _shakeIntensity = 2f, _shakeDuration = 0.2f;
    private Vector3 _startPos;
    void Start()
    {
        _timerRect = timerText.GetComponent<RectTransform>();
        _startPos = _timerRect.localPosition;
        _gm = GameManager.instance;
    }

    void Update()
    {
        if(!_gm.IsGamePlaying()) return;
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timeRemaining = Mathf.Max(timeRemaining, 0);
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                GameManager.instance.GameOver();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if(timeRemaining < 61f)
        {
            if(seconds > 10f) timerText.color = _warningTimer1;
            else
            {
                timerText.color = _warningTimer2;
                if(seconds <= 5f)
                {
                    ShakeText();
                }
            }
        }

    }
    private void ShakeText()
    {
        LeanTween.cancel(_timerRect.gameObject);
        LeanTween.moveLocal(_timerRect.gameObject, _startPos + (Vector3)Random.insideUnitCircle * _shakeIntensity, _shakeDuration).setEase(LeanTweenType.easeShake).setLoopPingPong(3).setOnComplete(
            ()=> {_timerRect.localPosition = _startPos; Debug.Log("SHAKEEEE");}
        );
    }
}