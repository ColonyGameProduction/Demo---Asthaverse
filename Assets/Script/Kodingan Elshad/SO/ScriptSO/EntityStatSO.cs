using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "EntityStatSO", menuName = "ScriptableObject/EntityStatSO")]
public class EntityStatSO : ScriptableObject
{
    public string entityName;
    public float health;
    public float speed;
    public float armor;
    public float acuracy;
    public float stealth;
    public float FOVRadius;

    public WeaponStatSO[] weaponStat;
}
