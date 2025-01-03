using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour
{
    public int totalQuest;
    public List<Quest> questList = new List<Quest>();
    public static QuestHandler questHandler;

    private void Awake()
    {
        questHandler = this;
    }
}
