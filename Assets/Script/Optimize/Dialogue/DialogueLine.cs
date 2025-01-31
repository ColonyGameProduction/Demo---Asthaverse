using System.Collections;

using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueLine : DialogueBase
    {


        [Header("Character")]
        protected IEnumerator _dialogue;



        
        public void GetTextContainer(TMP_Text textContainer)
        {
            _textContainer = textContainer;
            // Debug.Log(textContainer.name);
        }

        public virtual void SetDialogue(Dialogue_Lines dialogueInput, DialogueCharacter character)
        {

            _textContainer.text = character.charaFullName + ": ";

            _textContainer.color = character.charaDialogueColor;
            // textHolder.font = textFont;

            _dialogue = typeText(dialogueInput.dialogueLine, dialogueInput.delayTypeText, dialogueInput.delayTypeBetweenLines);

            // Debug.Log("???????");
            StartCoroutine(_dialogue);
        }
        public virtual void StopDialogue()
        {
            if(_dialogue == null) return;

            StopCoroutine(_dialogue);
            _dialogue = null;
            _finished = false;
        }
    }

}
