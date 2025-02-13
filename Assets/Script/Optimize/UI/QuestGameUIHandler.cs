using System;

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class QuestContainerUI
{
    public QuestContainerUI(GameObject parent, Image bg, Image strip, TextMeshProUGUI textUI, string questOptionalSentences)
    {
        containerParent = parent;
        parentHLayoutGroup = containerParent.GetComponent<HorizontalLayoutGroup>();
        containerBG = bg;
        stripPointContainer = strip.gameObject;

        text = textUI;
        questOptionalText = questOptionalSentences;

    }
    public GameObject containerParent;
    public HorizontalLayoutGroup parentHLayoutGroup;
    public TextMeshProUGUI text;
    public Image containerBG;
    public GameObject stripPointContainer;
    public QuestHandlerParent quest;
    public bool isMultipleQuest;
    public bool isOptional;
    private string questOptionalText;
    public void ChangeText()
    {
        if(quest == null) return;

        text.text = quest.QuestDesc;

        if(isOptional) text.text += " " + questOptionalText;
    }


}
public class QuestGameUIHandler : MonoBehaviour
{
    public static QuestGameUIHandler Instance {get; private set;}
    [SerializeField] private GameObject _questContainerObject;
    [SerializeField] private GameObject _questContainerParent;
    [SerializeField] private GameObject _questContainerPrefab;
    [SerializeField] private int _totalQuestContainerNeeded = 10;
    private List<QuestContainerUI> _questContainerList = new List<QuestContainerUI>();

    [Header("Data")]
    [Header("Text")]
    [FormerlySerializedAs("_startColor")][SerializeField] private Color _startTextColor;
    [FormerlySerializedAs("_completeColor")][SerializeField] private Color _completeTextColor;
    [SerializeField] private TMP_FontAsset _soloQuestFont, _multipleQuestFont;
    [SerializeField] private float _soloQuestFontSize, _multipleQuestFontSize;
    [SerializeField] private float _changeTextAlphaDuration = 0.5f;
    [TextArea(2,3)][SerializeField] private string _questOptionalText;
    [SerializeField] private int _rightPaddingParent = 30;

    [Header("Container BG")]
    [SerializeField] private Color _startBGColor;
    [SerializeField] private Color _completeBGColor;
    private int _currActivatedChildIdx;
    private void Awake() 
    {
        Instance = this;
        CreateQuestContainers();
        ResetCurrActivatedChildIdx();
    }

    public void ShowQuestContainerUI()
    {
        if(_questContainerObject.activeSelf) return;
        _questContainerObject.SetActive(true);
    }
    public void HideQuestContainerUI()
    {
        if(!_questContainerObject.activeSelf) return;
        _questContainerObject.SetActive(false);
    }
    public void CreateQuestContainers()
    {
        for(int i=0; i < _totalQuestContainerNeeded; i++)
        {
            GameObject newQuestContainer = Instantiate(_questContainerPrefab, _questContainerParent.transform);
            newQuestContainer.SetActive(false);

            Image bgContainer = newQuestContainer.GetComponent<Image>();
            Image stripPoint = newQuestContainer.transform.GetChild(0).GetComponent<Image>();

            TextMeshProUGUI text = newQuestContainer.GetComponentInChildren<TextMeshProUGUI>();
            text.color = _startTextColor;


            QuestContainerUI questContainerUI = new QuestContainerUI(newQuestContainer, bgContainer, stripPoint, text, _questOptionalText);
            _questContainerList.Add(questContainerUI);
        }
    }

    public void CallQuestContainer(QuestHandlerParent quest, bool isMultipleQuest, bool isOptional)
    {
        QuestContainerUI questContainerUI = GetUnusedContainer();
        if(questContainerUI == null) return;

        questContainerUI.quest = quest;
        questContainerUI.isMultipleQuest = isMultipleQuest;
        questContainerUI.isOptional = isOptional;
        questContainerUI.text.font = isMultipleQuest ? _multipleQuestFont : _soloQuestFont;
        questContainerUI.text.fontSize = isMultipleQuest ? _multipleQuestFontSize : _soloQuestFontSize;
        questContainerUI.parentHLayoutGroup.padding.right = isMultipleQuest ? _rightPaddingParent : 0;

        if(!isMultipleQuest)
        {
            SoloQuestHandler soloQuest = quest as SoloQuestHandler;
            soloQuest.OnChangeDescBeforeComplete += questContainerUI.ChangeText;
            ChangeImageAlphaValue(questContainerUI.containerBG, 1);
        }
        else
        {
            ChangeImageAlphaValue(questContainerUI.containerBG, 0);
            questContainerUI.stripPointContainer.SetActive(false);
        }
        

        ChangeTextAlphaValue(questContainerUI.text, 1);
        questContainerUI.text.color = _startTextColor;
        questContainerUI.containerBG.color = _startBGColor;
        questContainerUI.ChangeText();

        questContainerUI.containerParent.transform.SetSiblingIndex(_currActivatedChildIdx);
        _currActivatedChildIdx++;
        questContainerUI.containerParent.SetActive(true);
        
        // questContainerUI.stripPointContainer.SetActive(isMultipleQuest ? false : true);
    }

    public void HideCompletedQuestContainer(QuestHandlerParent quest)
    {
        QuestContainerUI questContainerUI = GetUsedContainer(quest);
        if(questContainerUI == null) return;

        if(!questContainerUI.isMultipleQuest)
        {
            SoloQuestHandler soloQuest = quest as SoloQuestHandler;
            soloQuest.OnChangeDescBeforeComplete -= questContainerUI.ChangeText;
        }
        questContainerUI.quest = null;
        questContainerUI.isMultipleQuest = false;
        questContainerUI.isOptional = false;
    

        questContainerUI.containerBG.color = _completeTextColor;
        questContainerUI.text.color = _completeTextColor;
    
        LeanTween.value(1, 0, _changeTextAlphaDuration).setOnUpdate((float value) =>
        {
            ChangeTextAlphaValue(questContainerUI.text, value);
        }).
        setOnComplete(
            ()=>
            {
                questContainerUI.containerParent.SetActive(false);
                ChangeImageAlphaValue(questContainerUI.containerBG, 1);
                questContainerUI.stripPointContainer.SetActive(true);
            }
        );
    }
    private QuestContainerUI GetUnusedContainer()
    {
        // QuestContainerUI container = _que
        foreach(QuestContainerUI container in _questContainerList)
        {
            if(container.quest == null && !container.containerParent.activeSelf) return container;
        }
        return null;
    }
    private QuestContainerUI GetUsedContainer(QuestHandlerParent quest)
    {
        foreach(QuestContainerUI container in _questContainerList)
        {
            if(container.quest == quest) return container;
        }
        return null;
    }

    private void ChangeTextAlphaValue(TextMeshProUGUI text, float value)
    {
        Color color = text.color;
        color.a = value;
        text.color = color;
    }
    private void ChangeImageAlphaValue(Image image, float value)
    {
        Color color = image.color;
        color.a = value;
        image.color = color;

        if(value == 0 || value == 1)
        {
            _startBGColor.a = value;
            _completeBGColor.a = value;
        }
    }

    public void ResetCurrActivatedChildIdx()
    {
        // Debug.Log("Hayooo");
        _currActivatedChildIdx = 0;
    }
}
