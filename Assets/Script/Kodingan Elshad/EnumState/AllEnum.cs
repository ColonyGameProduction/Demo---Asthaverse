using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//untuk armor type
public enum armourType
{
    Light = 20,
    Medium = 30,
    Heavy = 50,
}

//untuk weapon type
public enum weaponType
{
    Pistol,
    AssaultRifle,
    Shotgun,
    Sniper,
    SMG,
    Rifle,
    MachineGun,
}

///summary
///Enum ini penanda, karakter saat ini lg kondisi dikontrol AI atau dikontrol Player atau ga
//untuk alert system enemy
public enum alertState
{
    Idle,
    Hunted,
    Engage,
}

public enum bodyParts
{
    head,
    body,
    leg,
    None
}
public enum FOVDistState
{
    none,
    far,
    middle,
    close
}
[Serializable]
public enum GameState
{
    BeforeStart,
    Play,
    Cinematic,
    Pause,
    Finish,
    GameOver
}
[Serializable]
public enum GamePlayMode
{
    Normal, Event, Switching
}
[Serializable]
public enum GamePauseState
{
    None,
    PauseMenu,
    Settings
}

[Serializable]
public enum DialogCutsceneTitle
{
    None, Test1, Test2, Cutscene1, 
    Ext_Compound1, Int_Compound, Int_Tourism1, 
    Int_Tourism2, Int_Tourism3, Int_Tourism4, 
    Int_Tourism5, Ext_Tourism, 
    Ext_CommandCentre, Int_CommandCentre, 
    Armory, Qst_Eavesdrop,

    Int_AbbandonVillage1, Int_AbbandonVillage2, Int_AbbandonVillage3,
    Ext_Compound2, Int_VehicleDepot1, Int_VehicleDepot2, Int_TrainingArea1,
    Int_TrainingArea2, Int_Barracks, 
    Ext_HQ1, Ext_HQ2, Ext_HQ3
}

public enum KeybindUIType
{
    General, PickUp, Command 
}

[Serializable]
public enum QuestName
{
    None, KillEnemy, Turn_Off_Alarm, Go_To_Position, Eavesdrop, Investigate_Truck_Schedule, Investigate_Patrol_Schedule, 
    Sabotage_Truck, Sabotage_Training_Equipment, Investigate_Barrack_Schedule, Investigate_Vehicle_Depot, Investigate_Abbandon_Village,
    Investigate_Training_Camp, Disable_Alarm, Lay_Traps, Investigate_Barracks,
    Revive_CrawlingFriend
}
