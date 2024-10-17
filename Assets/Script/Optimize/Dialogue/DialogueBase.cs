using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueBase : MonoBehaviour
    {
        [SerializeField] protected int _startVAIDX = 1;
        protected TMP_Text _textContainer;
        protected bool _finished;
        private bool isRichText = false;
        private string saveRichText;

        /// Kalo mo tambahin sfx tengah-tengah bisa pake isrichtext juga modelannya :D, tp bikin penanda sendiri; delay jg bisa
        /// kek misal <sfx: sfxname> or <delay: 0.2f>
        
        public bool Finished { get { return _finished;}}
        public IEnumerator typeText(string inputText, float delayTypeText, float delayBetweenLines)
        {
            
            isRichText = false;
            
            for(int i=0; i<inputText.Length;i++)
            {
                
                // if(i == _startVAIDX && vaAudioName != ""){
                //     //Play VA audio
                // }

                if(inputText[i] == '<' && !isRichText)
                {
                    isRichText = true;
                    // isRichTextDone = false;
                    saveRichText = "";
                }
                if(isRichText)
                {
                    saveRichText += inputText[i];
                    if(inputText[i] == '>')
                    {
                        isRichText = false;
                        _textContainer.text += saveRichText;
                    }
                    continue;
                }
                

                _textContainer.text += inputText[i];

                // Debug.Log(inputText[i] + " " + i);
                yield return new WaitForSeconds(delayTypeText);
                
            }
            
            yield return new WaitForSeconds(delayBetweenLines);
            
            
            _finished = true;
            // Debug.Log("this dialog done");
                        
        }

        public void ChangeFinished_false()
        {
            _finished = false;
            // Debug.Log("fakse lg");
        }


    }

}
