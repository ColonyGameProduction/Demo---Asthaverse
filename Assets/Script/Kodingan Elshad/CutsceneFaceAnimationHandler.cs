using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class CutsceneFaceAnimationHandler : MonoBehaviour
{
    public List<Canvas> faces = new List<Canvas>();

    public VideoPlayer curVideo;
    private Sprite curSprite;
    public CutsceneMouthSO cutsceneMouth;
    public DialogCutsceneSO curCutsceneDialog;
    public TextMeshProUGUI leftName;
    public TextMeshProUGUI rightName;
    public TextMeshProUGUI text;

    public float textSpeed;
    public int prevCharID;
    public int charID;

    private int startCharID;
    private int index;
    private bool first;

    private void Start()
    {
        leftName.text = string.Empty;
        rightName.text = string.Empty;
        text.text = string.Empty;

        for (int i = 0; i < faces.Count; i++)
        {
            LeanTween.alpha(faces[i].GetComponentInChildren<RawImage>().rectTransform, 0, 0);
            
        }

        first = true;
        AssigningTheFace();
        AddingTheWord();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (text.text == curCutsceneDialog.dialogSentence[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                text.text = curCutsceneDialog.dialogSentence[index];
            }
        }
    }

    public void AssigningName()
    {

        if (first)
        {

            LeanTween.alpha(faces[0].GetComponentInChildren<RawImage>().rectTransform, 1, .7f);

                startCharID = curCutsceneDialog.charID[index] - 1;
                leftName.text = curCutsceneDialog.character[startCharID].Name;


            LeanTween.alpha(faces[1].GetComponentInChildren<RawImage>().rectTransform, 1, .7f);
                startCharID = curCutsceneDialog.charID[index + 1] - 1;
                rightName.text = curCutsceneDialog.character[startCharID].Name;
                StartCoroutine(TypeLine());
            if (charID % 2 == 0)
            {
                LeanTween.value(.1f, 1, 0.5f).setOnUpdate((float value) => {
                    Color color = leftName.color;
                    color.a = value;
                    leftName.color = color;
                });
                LeanTween.value(1, .1f, 0.5f).setOnUpdate((float value) => {
                    Color color = rightName.color;
                    color.a = value;
                    rightName.color = color;
                });
            }
            else
            {
                LeanTween.value(.1f, 1, 0.5f).setOnUpdate((float value) => {
                    Color color = rightName.color;
                    color.a = value;
                    rightName.color = color;
                });
                LeanTween.value(1, .1f, 0.5f).setOnUpdate((float value) => {
                    Color color = leftName.color;
                    color.a = value;
                    leftName.color = color;
                });
            }

            faces[curCutsceneDialog.charID[index]].GetComponentInChildren<VideoPlayer>().Play();
            faces[curCutsceneDialog.charID[index]].GetComponentInChildren<VideoPlayer>().Pause();
            first = false;
            
        }
        else
        {
            if (charID % 2 == 0)
            {
                leftName.text = curCutsceneDialog.character[charID].Name;
                LeanTween.value(.1f, 1, 0.5f).setOnUpdate((float value) => {
                    Color color = leftName.color;
                    color.a = value;
                    leftName.color = color;
                });

                float decreaseValue = rightName.color.a;
                Debug.Log(decreaseValue);

                LeanTween.value(decreaseValue, .1f, 0.5f).setOnUpdate((float value) => {
                    if(decreaseValue > .1f)
                    {
                        Color color = rightName.color;
                        color.a = value;
                        rightName.color = color;
                    }                    
                });
            }
            else
            {
                rightName.text = curCutsceneDialog.character[charID].Name;
                LeanTween.value(.1f, 1, 0.5f).setOnUpdate((float value) => {
                    Color color = rightName.color;
                    color.a = value;
                    rightName.color = color;
                });
                
                float decreaseValue = leftName.color.a;
                Debug.Log(decreaseValue);

                LeanTween.value(decreaseValue, .1f, 0.5f).setOnUpdate((float value) => {
                    if (decreaseValue > .1f)
                    {
                        Debug.Log("Masuk");
                        Color color = leftName.color;
                        color.a = value;
                        leftName.color = color;
                    }
                    
                });
            }
        }
    }

    public void AssigningTheFace()
    {
        for (int i = 0; i < curCutsceneDialog.character.Count; i++) 
        {
            curVideo = faces[i].GetComponentInChildren<VideoPlayer>();
            curVideo.clip = curCutsceneDialog.character[i].MouthAnimation;
        }

    }

    public void MovingTheMouth()
    {
        faces[charID].gameObject.SetActive(true);

        LeanTween.alpha(faces[charID].GetComponentInChildren<RawImage>().rectTransform, 1, .5f);

        LeanTween.move(faces[charID].GetComponentInChildren<RawImage>().gameObject, faces[charID].GetComponentInChildren<RawImage>().gameObject.transform.position + new Vector3(-15, 10, 0), .5f);
        StartCoroutine(LeanDelay());
        LeanTween.alpha(faces[charID].GetComponentInChildren<Image>().rectTransform, 0, .5f);

        faces[charID].GetComponentInChildren<VideoPlayer>().Play();


        if (charID != prevCharID)
        {
            LeanTween.move(faces[prevCharID].GetComponentInChildren<RawImage>().gameObject, faces[prevCharID].GetComponentInChildren<RawImage>().gameObject.transform.position + new Vector3(15, -10, 0), .5f);

            LeanTween.alpha(faces[prevCharID].GetComponentInChildren<Image>().rectTransform, .9f, .5f);

            faces[prevCharID].GetComponentInChildren<VideoPlayer>().Pause();
        }

    }

    public void NextLine()
    {
        if(index < curCutsceneDialog.dialogSentence.Count - 1)
        {
            index++;
            AddingTheWord();
        }
        else if(index ==  curCutsceneDialog.dialogSentence.Count - 1)
        {
            Debug.Log("Cutscene Clear");
        }
    }

    public void AddingTheWord()
    {
        text.text = string.Empty;
        prevCharID = charID;
        charID = curCutsceneDialog.charID[index] - 1;
        if(!first)
        {
            StartCoroutine(TypeLine());
        }
        MovingTheMouth();
        AssigningName();
    }

    IEnumerator TypeLine()
    {
        foreach (char c in curCutsceneDialog.dialogSentence[index].ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator LeanDelay()
    {
        yield return new WaitForSeconds(.3f);

        if(charID != prevCharID)
        {
            faces[prevCharID].sortingOrder = 1;
        }
        faces[charID].sortingOrder = 2;
        
    }

}