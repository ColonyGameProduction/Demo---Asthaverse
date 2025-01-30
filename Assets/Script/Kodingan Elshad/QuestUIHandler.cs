using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUIHandler : MonoBehaviour
{
    public static QuestUIHandler instance;
    public TMP_FontAsset questFont;
    public List<TextMeshProUGUI> mainQuest = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> sideQuestName = new List<TextMeshProUGUI>();
    public RectTransform questUIStartTransform;
    public GameObject questUI;
    public int uiSpace = 0;
    public int sideQuestCounter;
    public int mainQuestCounter;
    public string sideQuestTempWords;
    public string mainQuestTempWords;

    public void Awake()
    {
        instance = this;
    }

    public void CreatingQuestUI(string text, Quest currQuest)
    {
        TextMeshProUGUI questText = Instantiate(questUI, questUIStartTransform).GetComponentInChildren<TextMeshProUGUI>();
        questText.gameObject.GetComponent<RectTransform>().position = questUIStartTransform.GetComponent<RectTransform>().position;

        questText.fontSize = 30;
        questText.font = questFont;
        questText.text = text;


        if (!currQuest.isOptional)
        {
            // Debug.Log(questText.text);
            mainQuest.Add(questText);
            if (questText.text == mainQuestTempWords)
            {
                mainQuestCounter++;
                mainQuest.Remove(questText);
                Destroy(questText.transform.parent.gameObject);
                // Debug.Log(mainQuestCounter.ToString());
            }
            else
            {
                mainQuestTempWords = "";
                mainQuestCounter = 1;
            }
            mainQuestTempWords = questText.text;
            if(mainQuestCounter > 1)
            {
                foreach (var item in mainQuest)
                {
                    item.text = mainQuestTempWords;
                    item.text += "(" + mainQuestCounter.ToString() + ")";
                }
            }
        }
        else
        {
            sideQuestName.Add(questText);
            if (questText.text == sideQuestTempWords)
            {
                sideQuestCounter++;
                sideQuestName.Remove(questText);
                Destroy(questText.transform.parent.gameObject);
                // Debug.Log(sideQuestCounter.ToString());
            }
            else
            {
                sideQuestCounter = 1;
            }
            sideQuestTempWords = questText.text;
            if(sideQuestCounter > 1)
            {
                foreach(var item in sideQuestName)
                {
                    item.text = sideQuestTempWords;
                    item.text += "(optional)";
                    item.text += "(" + sideQuestCounter.ToString() + ")";
                }
            }
        }
    }

    public void RemovingQuestUI(string checker, Quest currQuest)
    {
        if (!currQuest.isOptional)
        {
            foreach (var item in mainQuest)
            {
                if (item.text.Contains(checker))
                {
                    LeanTween.value(1, 0, 1f).setOnUpdate((float value) =>
                    {
                        Color color = item.color;
                        color.a = value;
                        item.color = color;
                    }).setOnComplete(() =>
                    {
                        mainQuest.Remove(item);
                        Destroy(item.transform.parent.gameObject);
                    });
                }
            }
        }
        else
        {
            foreach (var item in sideQuestName)
            {
                // Debug.Log(item.text + " masuk " + checker);
                if (item.text.Contains("(optional)"))
                {
                    // Debug.Log("tai");
                    if (item.text.Contains(checker))
                    {
                        // Debug.Log("tai2");
                        LeanTween.value(1, 0, 1f).setOnUpdate((float value) =>
                        {
                            Color color = item.color;
                            color.a = value;
                            item.color = color;
                        }).setOnComplete(() =>
                        {
                            mainQuest.Remove(item);
                            Destroy(item.transform.parent.gameObject);
                        });
                    }
                }
            }
        }
    }
}
