using System.Collections;

using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private SODialogueCharacterList _SOdialogueCharaList;

        [Header("Dialogue Line")]
        [SerializeField] private DialogueLine _dialogueLineContainer;

        [Header("Containers")]
        [SerializeField] private GameObject _bgContainer;
        [SerializeField] private TMP_Text _textContainer;

        [Tooltip("If true, langsung hide, if not true, tunggu Action apa baru tutup dr sana")]
        [SerializeField]private bool hasSceneDialogueFinish;

        [SerializeField]IEnumerator dialogSeq;
        private void Awake() 
        {
            if(_dialogueLineContainer == null) _dialogueLineContainer = GetComponent<DialogueLine>();
            HideDialogue();
        }

        private IEnumerator dialogueSequence(SODialogues SceneDialogue)
        {
            _dialogueLineContainer.GetTextContainer(_textContainer);
            
            for(int i=0;i<SceneDialogue.dialogue_Lines.Count;i++)
            {
                if(!_dialogueLineContainer.gameObject.activeSelf)_dialogueLineContainer.gameObject.SetActive(true);
                
                Dialogue_Lines dialogueNow = SceneDialogue.dialogue_Lines[i];
                
                int dialogueTalkerNow = (int)dialogueNow.charaName;

                _dialogueLineContainer.SetDialogue(dialogueNow, _SOdialogueCharaList.SearchCharacter(dialogueTalkerNow));

                yield return new WaitUntil(()=> _dialogueLineContainer.Finished);
                _dialogueLineContainer.ChangeFinished_false();
                
            }
            hasSceneDialogueFinish = true;
            HideDialogue();

            // if(DialogueManager.DoSomethingAfterFinish != null)DialogueManager.DoSomethingAfterFinish();
        }
        private void Deactivate()
        {
            _dialogueLineContainer.gameObject.SetActive(false);
        }
        public void ShowDialogue(SODialogues SceneDialogue)
        {

            StopCourotineNow();
            _bgContainer.SetActive(true);
            dialogSeq = dialogueSequence(SceneDialogue);

            if(dialogSeq != null)StartCoroutine(dialogSeq);
            else
            {
                // Debug.Log("WHY IT'S NULL");
                // Debug.Log(SceneDialogue);
            }
            
        }
        public void HideDialogue()
        {
            if(hasSceneDialogueFinish)hasSceneDialogueFinish = false;
            _bgContainer.SetActive(false);
            _textContainer.text = "";
            // gameObject.SetActive(false);
            
        }
        public void StopCourotineNow()
        {
            if(!_dialogueLineContainer.Finished)_dialogueLineContainer.StopDialogue();
            if(dialogSeq == null)return;
            if(!hasSceneDialogueFinish)StopCoroutine(dialogSeq);
            dialogSeq = null;
            
        }

        public void StopCoroutineAbruptly()
        {
            StopCourotineNow();
            HideDialogue();

        }

        public bool HasSceneDialogueFinish() {return hasSceneDialogueFinish;}

        // public void GetContainer()
    }
}
