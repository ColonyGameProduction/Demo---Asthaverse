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

//untuk alert system enemy
public enum alertState
{
    Idle,
    Hunted,
    Engage,
}

public enum FOVDistState
{
    far,
    middle,
    close,
}