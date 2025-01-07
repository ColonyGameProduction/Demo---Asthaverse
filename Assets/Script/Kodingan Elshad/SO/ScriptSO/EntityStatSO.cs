using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "EntityStatSO", menuName = "ScriptableObject/EntityStatSO")]
public class EntityStatSO : ScriptableObject
{
    public string entityName;
    public bool isCharBoy;
    public float health;
    public float speed;
    public float armor;
    public armourType armourType;
    public float acuracy;
    public float stealth;
    public float FOVRadius;

    public WeaponStatSO[] weaponStat;

    public Sprite cropImage;
}
