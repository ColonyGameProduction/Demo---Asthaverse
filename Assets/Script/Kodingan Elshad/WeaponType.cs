using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
    [SerializeField] private string weaponType;
    public string Weapon()
    {
        return weaponType;
    }
}
