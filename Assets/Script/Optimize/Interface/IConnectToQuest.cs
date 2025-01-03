using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConnectToQuest
{
    event Action OnTriggerQuestComplete;
}

