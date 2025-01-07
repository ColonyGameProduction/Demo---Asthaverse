using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestContainerUI
{
    public QuestContainerUI(GameObject parent, TextMeshProUGUI textUI, string questOptionalSentences)
    {
        textParent = parent;
        text = textUI;
        questOptionalText = questOptionalSentences;

    }
    public GameObject textParent;
    public TextMeshProUGUI text;
    public SoloQuestHandler quest;
    private string questOptionalText;
    public void ChangeText()
    {
        if(quest == null) return;

        text.text = "- " + quest.QuestDesc;
        if(quest.IsOptional) text.text += " " + questOptionalText;
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
    [SerializeField] private Color _startColor;
    [SerializeField] private Color _completeColor;
    [SerializeField] private float _changeTextAlphaDuration = 0.5f;
    [TextArea(2,3)][SerializeField] private string _questOptionalText;
    private int _currIdx;
    private void Awake() 
    {
        Instance = this;
        CreateQuestContainers();
        _currIdx = 0;
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

            TextMeshProUGUI text = newQuestContainer.GetComponentInChildren<TextMeshProUGUI>();
            text.color = _startColor;

            QuestContainerUI questContainerUI = new QuestContainerUI(newQuestContainer, text, _questOptionalText);
            _questContainerList.Add(questContainerUI);
        }
    }

    public void CallQuestContainer(SoloQuestHandler quest)
    {
        QuestContainerUI questContainerUI = GetUnusedContainer();
        if(questContainerUI == null) return;

        questContainerUI.quest = quest;
        quest.OnChangeDescBeforeComplete += questContainerUI.ChangeText;

        ChangeTextAlphaValue(questContainerUI.text, 1);
        questContainerUI.text.color = _startColor;
        questContainerUI.ChangeText();

        questContainerUI.textParent.SetActive(true);
    }

    public void HideCompletedQuestContainer(SoloQuestHandler quest)
    {
        QuestContainerUI questContainerUI = GetUsedContainer(quest);
        if(questContainerUI == null) return;

        quest.OnChangeDescBeforeComplete -= questContainerUI.ChangeText;
        questContainerUI.quest = null;
        
        questContainerUI.text.color = _completeColor;
    
        LeanTween.value(1, 0, _changeTextAlphaDuration).setOnUpdate((float value) =>
        {
            ChangeTextAlphaValue(questContainerUI.text, value);
        }).
        setOnComplete(
            ()=>
            {
                questContainerUI.textParent.SetActive(false);
            }
        );
    }
    private QuestContainerUI GetUnusedContainer()
    {
        // QuestContainerUI container = _que
        foreach(QuestContainerUI container in _questContainerList)
        {
            if(container.quest == null && !container.textParent.activeSelf) return container;
        }
        return null;
    }
    private QuestContainerUI GetUsedContainer(SoloQuestHandler quest)
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

}
