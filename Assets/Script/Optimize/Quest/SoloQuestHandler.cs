using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloQuestHandler : QuestHandlerParent
{
    [SerializeField] private bool _isOptional;
    #region  Getter Setter
    public bool IsOptional {get {return _isOptional;}}
    #endregion
}
