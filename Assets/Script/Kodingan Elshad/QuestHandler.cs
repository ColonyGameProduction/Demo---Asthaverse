using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour
{
    public int totalQuest;
    public List<T> questList = new List<T>();
    public static QuestHandler questHandler;

    private void Awake()
    {
        questHandler = this;
    }
}
