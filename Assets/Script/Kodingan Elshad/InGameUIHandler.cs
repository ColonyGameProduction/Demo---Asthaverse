using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class InGameUIHandler : MonoBehaviour
{
    GameManager gm;

    int index;

    public List<GameObject> characterUI = new List<GameObject>();
    public VideoPlayer video;
    public TextMeshProUGUI dialogText;
    public DialogCutsceneSO dialogCutscene;
    private bool canNextDialog;


    private void Start()
    {
        gm = GameManager.instance;
        StartingTheUICharacter();
    }

    public void StartingTheUICharacter()
    {
        for (int i = 0; i < gm.playerGameObject.Length; i++)
        {
            characterUI[i].SetActive(true);
            gm.playerGameObject[i].GetComponent<PlayerAction>().enabled = true;
            characterUI[i].GetComponent<InGameUICharHandler>().AssigningCharUI(gm.playerGameObject[i]);
            if (i != 0)
            {
                gm.playerGameObject[i].GetComponent<PlayerAction>().enabled = false;
            }
        }
    }

    public void SwitchingUICharacter()
    {

        if(gm.playerGameObject.Length < 3)
        {
            switch (gm.playableCharacterNum)
            {
                case 0:
                    LeanTween.scale(characterUI[0], characterUI[1].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, .2f);

                    LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(160, 80, 0), .2f);
                    LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(-160, -80, 0), .2f);

                    LeanTween.moveLocal(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(80, 30, 0), .2f);
                    LeanTween.scale(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1.5f, 1.5f, 1.5f), .2f);

                    LeanTween.moveLocal(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(-80, -30, 0), .2f);
                    LeanTween.scale(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1f, 1f, 1f), .2f);

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

                    characterUI[1].GetComponent<InGameUICharHandler>().AssigningCharUI(gm.playerGameObject[1]);

                    break;
                case 1:
                    LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[0], characterUI[1].GetComponent<RectTransform>().localScale, .2f);

                    LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(160, 80, 0), .2f);
                    LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(-160, -80, 0), .2f);

                    LeanTween.moveLocal(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(80, 30, 0), .2f);
                    LeanTween.scale(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1.5f, 1.5f, 1.5f), .2f);

                    LeanTween.moveLocal(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(-80, -30, 0), .2f);
                    LeanTween.scale(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1f, 1f, 1f), .2f);

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

                    characterUI[0].GetComponent<InGameUICharHandler>().AssigningCharUI(gm.playerGameObject[0]);

                    break;
            }
        }
        else
        {
            switch (gm.playableCharacterNum)
            {
                case 0:
                    LeanTween.scale(characterUI[0], characterUI[2].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[2], characterUI[1].GetComponent<RectTransform>().localScale, .2f);

                    LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(160, 135, 0), .2f);
                    LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(-160, -80, 0), .2f);
                    LeanTween.move(characterUI[2], characterUI[2].transform.position + new Vector3(0, -55, 0), .2f);

                    LeanTween.moveLocal(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(80, 30, 0), .2f);
                    LeanTween.scale(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1.5f, 1.5f, 1.5f), .2f);

                    LeanTween.moveLocal(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(-80, -30, 0), .2f);
                    LeanTween.scale(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1f, 1f, 1f), .2f);

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

                    characterUI[1].GetComponent<InGameUICharHandler>().AssigningCharUI(gm.playerGameObject[1]);

                    break;
                case 1:
                    LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[2], characterUI[1].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[0], characterUI[2].GetComponent<RectTransform>().localScale, .2f);

                    LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(160, 135, 0), .2f);
                    LeanTween.move(characterUI[2], characterUI[2].transform.position + new Vector3(-160, -80, 0), .2f);
                    LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(0, -55, 0), .2f);

                    LeanTween.moveLocal(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(80, 30, 0), .2f);
                    LeanTween.scale(characterUI[1].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1.5f, 1.5f, 1.5f), .2f);

                    LeanTween.moveLocal(characterUI[2].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[2].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(-80, -30, 0), .2f);
                    LeanTween.scale(characterUI[2].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1f, 1f, 1f), .2f);

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);

                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

                    characterUI[2].GetComponent<InGameUICharHandler>().AssigningCharUI(gm.playerGameObject[2]);

                    break;
                case 2:
                    LeanTween.scale(characterUI[2], characterUI[1].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[0], characterUI[2].GetComponent<RectTransform>().localScale, .2f);
                    LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, .2f);

                    LeanTween.move(characterUI[2], characterUI[2].transform.position + new Vector3(160, 135, 0), .2f);
                    LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(-160, -80, 0), .2f);
                    LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(0, -55, 0), .2f);

                    LeanTween.moveLocal(characterUI[2].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[2].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(80, 30, 0), .2f);
                    LeanTween.scale(characterUI[2].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1.5f, 1.5f, 1.5f), .2f);

                    LeanTween.moveLocal(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).localPosition + new Vector3(-80, -30, 0), .2f);
                    LeanTween.scale(characterUI[0].transform.GetChild(0).GetChild(0).GetChild(2).gameObject, new Vector3(1f, 1f, 1f), .2f);

                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

                    characterUI[0].GetComponent<InGameUICharHandler>().AssigningCharUI(gm.playerGameObject[0]);

                    break;
            }
        }

        
    }

    public void DialogPlay()
    {
        video.clip = dialogCutscene.character[dialogCutscene.charID[index] - 1].MouthAnimation;
        dialogText.text = dialogCutscene.dialogSentence[index];

        LeanTween.alpha(video.gameObject.GetComponent<RectTransform>(), 1, .1f);
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float value) => {
            Color color = dialogText.color;
            color.a = value;
            dialogText.color = color;
        });

        StartCoroutine(NextDialog(3));

    }

    public IEnumerator NextDialog(float delay)
    {
        video.Play();
        canNextDialog = false;

        yield return new WaitForSeconds(delay);

        if (index == 0 || index == dialogCutscene.dialogSentence.Count - 1)
        {
            LeanTween.alpha(video.gameObject.GetComponent<RectTransform>(), 0, .1f);
        }
        else
        {
            if (dialogCutscene.charID[index] != dialogCutscene.charID[index + 1])
            {
                LeanTween.alpha(video.gameObject.GetComponent<RectTransform>(), 0, .1f);
            }
        }

        LeanTween.value(1, 0, 0.1f).setOnUpdate((float value) =>
        {
            Color color = dialogText.color;
            color.a = value;
            dialogText.color = color;
        }).setOnComplete(() =>
        {
            if (index != dialogCutscene.dialogSentence.Count - 1)
            {
                index++;
                DialogPlay();
            }
        });
    }

}
