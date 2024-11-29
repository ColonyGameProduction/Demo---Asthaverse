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
        }
    }

    public void SwitchingUICharacter()
    {
        switch (gm.playableCharacterNum)
        {
            case 0:
                LeanTween.scale(characterUI[0], characterUI[2].GetComponent<RectTransform>().localScale, 1f);
                LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, 1f);
                LeanTween.scale(characterUI[2], characterUI[1].GetComponent<RectTransform>().localScale, 1f);

                LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(44, 200, 0), 1f);
                LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(-22, -100, 0), 1f);
                LeanTween.move(characterUI[2], characterUI[2].transform.position + new Vector3(-22, -100, 0), 1f);

                break;
            case 1:
                LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, 1f);
                LeanTween.scale(characterUI[2], characterUI[1].GetComponent<RectTransform>().localScale, 1f);
                LeanTween.scale(characterUI[0], characterUI[2].GetComponent<RectTransform>().localScale, 1f);

                LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(44, 200, 0), 1f);
                LeanTween.move(characterUI[2], characterUI[2].transform.position + new Vector3(-22, -100, 0), 1f);
                LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(-22, -100, 0), 1f);

                break;
            case 2:
                LeanTween.scale(characterUI[2], characterUI[1].GetComponent<RectTransform>().localScale, 1f);
                LeanTween.scale(characterUI[0], characterUI[2].GetComponent<RectTransform>().localScale, 1f);
                LeanTween.scale(characterUI[1], characterUI[0].GetComponent<RectTransform>().localScale, 1f);

                LeanTween.move(characterUI[2], characterUI[2].transform.position + new Vector3(44, 200, 0), 1f);
                LeanTween.move(characterUI[0], characterUI[0].transform.position + new Vector3(-22, -100, 0), 1f);
                LeanTween.move(characterUI[1], characterUI[1].transform.position + new Vector3(-22, -100, 0), 1f);

                break;
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
