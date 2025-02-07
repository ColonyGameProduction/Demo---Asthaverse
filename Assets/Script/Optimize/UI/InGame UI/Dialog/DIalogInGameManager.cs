
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

[Serializable]
public class EventAfterDialogFinishFromInspector
{
    public DialogCutsceneTitle title;
    [ReadOnly(false)] public bool isDone;
    public UnityEvent action;
}
public class DIalogInGameManager : MonoBehaviour
{
    public static DIalogInGameManager Instance {get; private set;}
    [SerializeField] private DialogCutsceneSOList _dialogCutsceneList;
    [ReadOnly(false), SerializeField]private DialogCutsceneSO _chosenDialog;
    private int _currDialogIdx;
    private IEnumerator _currDialog;
    [SerializeField] private float _nextDialogDelay = 3;

    [Header("UI")]
    [SerializeField] private GameObject _background;
    [SerializeField] private Image _faceImage;
    private RectTransform _faceImageRectTransform;
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private TextMeshProUGUI _charName;
    [SerializeField] private VideoPlayer _video;
    [ReadOnly(false), SerializeField] private bool _canNextDialog;
    [Header("Event from inspector")]
    [SerializeField] private EventAfterDialogFinishFromInspector[] _eventAfterDialogFinishFromInspectors;
    private int _leanTweenDialogTextID;

    #region event
    public Action<DialogCutsceneTitle> OnDialogFinish;
    #endregion
    private void Awake() 
    {
        Instance = this;
        _faceImageRectTransform = _faceImage.gameObject.GetComponent<RectTransform>();
    }
    public void PlayDialogCutsceneString(string titleSameAsEnum)
    {
        StopCurrDialog();
        _chosenDialog = _dialogCutsceneList.SearchDialogCutSceneString(titleSameAsEnum);
        _currDialogIdx = 0;
        if(_chosenDialog != null) PlayDialog();
    }
    public void PlayDialogCutsceneTitle(DialogCutsceneTitle title)
    {
        StopCurrDialog();
        _chosenDialog = _dialogCutsceneList.SearchDialogCutScene(title);
        _currDialogIdx = 0;
        if(_chosenDialog != null) PlayDialog();
    }
    public void StopCurrDialog()
    {
        _background.gameObject.SetActive(false);
        ChangeTextAlphaValue(_dialogText, 0);
        _video.Stop();
        if(_currDialog != null)
        {
            StopCoroutine(_currDialog);
            // LeanTween.cancel(_faceImageRectTransform);
            _currDialog = null;
        }
        if(_chosenDialog != null) OnDialogFinish?.Invoke(_chosenDialog.title);
        _chosenDialog = null;
    }
    private void PlayDialog()
    {
        if(_chosenDialog == null) return;
        _background.gameObject.SetActive(true);
        _faceImage.sprite = _chosenDialog.character[_chosenDialog.charID[_currDialogIdx] - 1].cropFace;
        _charName.text = _chosenDialog.character[_chosenDialog.charID[_currDialogIdx] - 1].Name;
        _dialogText.text = _chosenDialog.dialogSentence[_currDialogIdx];
        _video.Play();

        LeanTween.alpha(_faceImageRectTransform, 1, .1f);
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float value) => {
            ChangeTextAlphaValue(_dialogText, value);
        });

        _currDialog = NextDialog(_nextDialogDelay);
        if(_chosenDialog != null)StartCoroutine(_currDialog);

    }
    public IEnumerator NextDialog(float delay)
    {
        _canNextDialog = false;

        yield return new WaitForSeconds(delay);

        if (_currDialogIdx == _chosenDialog.dialogSentence.Count - 1)
        {
            LeanTween.alpha(_faceImageRectTransform, 0, .1f);
            _background.gameObject.SetActive(false);
        }
        else
        {
            if (_chosenDialog.charID[_currDialogIdx] != _chosenDialog.charID[_currDialogIdx + 1])
            {
                LeanTween.alpha(_faceImageRectTransform, 0, .1f);
            }
        }

        LeanTween.value(1, 0, 0.1f).setOnUpdate((float value) =>
        {
            ChangeTextAlphaValue(_dialogText, value);
        }).setOnComplete(() =>
        {
            if(_chosenDialog != null)
            {
                _currDialog = null;
                if (_currDialogIdx != _chosenDialog.dialogSentence.Count - 1)
                {
                    _currDialogIdx++;
                    PlayDialog();
                }
                else
                {
                    OnDialogFinish?.Invoke(_chosenDialog.title);
                    CheckEventFromInspectorAfterDialogFinish(_chosenDialog.title);
                    StopCurrDialog();
                }
            }
            
        });
    }
    private void ChangeTextAlphaValue(TextMeshProUGUI chosen, float to)
    {
        to = Mathf.Clamp01(to);

        Color colorChosen = chosen.color;
        colorChosen.a = to;
        chosen.color = colorChosen;
    }
    private void CheckEventFromInspectorAfterDialogFinish(DialogCutsceneTitle title)
    {
        foreach(EventAfterDialogFinishFromInspector eventDialog in _eventAfterDialogFinishFromInspectors)
        {
            if(title == eventDialog.title && !eventDialog.isDone)
            {
                eventDialog.isDone = true;
                eventDialog.action?.Invoke();
            }
        }
    }
}
