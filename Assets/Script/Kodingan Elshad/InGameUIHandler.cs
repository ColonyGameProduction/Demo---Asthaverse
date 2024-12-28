using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class InGameUIHandler : MonoBehaviour
{
    GameManager gm;

    int index;

    public List<int> nextQuestID = new List<int>();
    public List<GameObject> characterUI = new List<GameObject>();
    public GameObject background;
    public Image faceImage;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI charName;
    public DialogCutsceneSO dialogCutscene;
    public VideoPlayer video;
    public Quest dialougeQuest;
    public bool canProceedToNextQuest;
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


    public void SwitchingWeaponUI(PlayerAction player)
    {
        Image weaponImage = characterUI[gm.playableCharacterNum].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        TextMeshProUGUI weaponName = weaponImage.GetComponentInChildren<TextMeshProUGUI>();
        GameObject weaponImageGameObject = weaponImage.gameObject;

        LeanTween.move(weaponImageGameObject, weaponImageGameObject.transform.position + new Vector3(200, 0, 0), .2f);
        LeanTween.alpha(weaponImageGameObject.GetComponent<RectTransform>(), 0, .2f)
        .setOnComplete(() =>
        {
            weaponImage.sprite = player.activeWeapon.gunShilouete;
            weaponName.text = player.activeWeapon.weaponName;

            LeanTween.move(weaponImageGameObject, weaponImageGameObject.transform.position + new Vector3(-200, 0, 0), .2f);
            LeanTween.alpha(weaponImageGameObject.GetComponent<RectTransform>(), 1, .2f);
        });

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

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

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

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

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

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

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

                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                    characterUI[1].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);
                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

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

                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                    characterUI[2].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);
                    characterUI[0].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

                    characterUI[0].GetComponent<InGameUICharHandler>().AssigningCharUI(gm.playerGameObject[0]);

                    break;
            }
        }

        
    }

    public void DialogPlay()
    {
        background.gameObject.SetActive(true);
        faceImage.sprite = dialogCutscene.character[dialogCutscene.charID[index] - 1].cropFace;
        charName.text = dialogCutscene.character[dialogCutscene.charID[index] - 1].Name;
        dialogText.text = dialogCutscene.dialogSentence[index];
        video.Play();

        LeanTween.alpha(faceImage.gameObject.GetComponent<RectTransform>(), 1, .1f);
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float value) => {
            Color color = dialogText.color;
            color.a = value;
            dialogText.color = color;
        });

        StartCoroutine(NextDialog(3));

    }

    public IEnumerator NextDialog(float delay)
    {
        canNextDialog = false;

        yield return new WaitForSeconds(delay);

        if (index == dialogCutscene.dialogSentence.Count - 1)
        {
            LeanTween.alpha(faceImage.gameObject.GetComponent<RectTransform>(), 0, .1f);
            if (dialougeQuest != null)
            {
                ActivatingNextQuest();
            }
            background.gameObject.SetActive(false);
        }
        else
        {
            if (dialogCutscene.charID[index] != dialogCutscene.charID[index + 1])
            {
                LeanTween.alpha(faceImage.gameObject.GetComponent<RectTransform>(), 0, .1f);
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

    public void ActivatingNextQuest()
    {
        dialougeQuest.questComplete = true;

        if (dialougeQuest.multiplyQuestAtOnce.Count > 0)
        {
            for (int i = 0; i < dialougeQuest.multiplyQuestAtOnce.Count; i++)
            {
                if (dialougeQuest.multiplyQuestAtOnce[i].questComplete == true)
                {
                    canProceedToNextQuest = true;
                }
                else
                {
                    canProceedToNextQuest = false;
                    break;
                }
            }
        }
        else
        {
            canProceedToNextQuest = true;
        }

        Debug.Log(canProceedToNextQuest);

        for (int i = 0; i < nextQuestID.Count; i++)
        {
            if (canProceedToNextQuest)
            {
                QuestHandler QH = QuestHandler.questHandler;
                Quest quest = QH.questList[nextQuestID[i]];
                quest.questActivate = true;
                quest.gameObject.SetActive(true);
                if (nextQuestID.Count > 1)
                {
                    for (int j = 0; j < nextQuestID.Count; j++)
                    {
                        quest.multiplyQuestAtOnce.Add(QH.questList[nextQuestID[j]]);
                    }
                }
            }
        }
    }

}
