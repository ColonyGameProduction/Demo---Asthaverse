
using UnityEngine;
using DialogueSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("Testing")]
    public bool isGO;
    public bool isStop;
    public DialogueSubTitle_Episode1 title;
    #region CanvasPart
    #endregion

    [Header("Component Variable")]
    [SerializeField] private SODialoguesList _comicDialogueList;
    [SerializeField] private DialogueHolder _dialogueHolder;
    private SODialogues _chosenDialogue;

    // [Header("Container")]
    // [SerializeField] private GameObject _bgContainer;
    // [SerializeField] private TMP_Text _textContainer, _nameTextContainer;
    private void Awake() 
    {
        if(_dialogueHolder == null)_dialogueHolder = GetComponent<DialogueHolder>();
    }
    private void Update() {
        if(isGO)
        {
            // Debug.Log("????");
            isGO = false;
            PlayDialogueScene(title);
        }
        if(isStop)
        {
            isStop = false;
            HideFinishedDialogueNow();
        }
    }
    public void PlayDialogueScene(DialogueSubTitle_Episode1 dialoguesTitle)
    {
        _chosenDialogue = _comicDialogueList.SearchDialogues(dialoguesTitle);
        if(_chosenDialogue != null)
        {
            _dialogueHolder.ShowDialogue(_chosenDialogue);
        }
    }
    public void HideFinishedDialogueNow()
    {
        _dialogueHolder.StopCoroutineAbruptly();
        _dialogueHolder.HideDialogue();
    }
    public void StopDialogue()
    {
        _dialogueHolder.StopCourotineNow();
    }
}
