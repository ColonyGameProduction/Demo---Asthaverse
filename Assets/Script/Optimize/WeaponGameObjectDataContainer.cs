using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponGameObjectData
{
    public string weaponName;
    public Transform _shootPlacement;
}
public class WeaponGameObjectDataContainer : MonoBehaviour
{
    [SerializeField] private List<WeaponGameObjectData> _weaponGameObjectDataList;

    public Transform GetShootPlacement(string weaponName)
    {
        foreach(WeaponGameObjectData weaponData in _weaponGameObjectDataList)
        {
            if(weaponName == weaponData.weaponName)return weaponData._shootPlacement;
        }
        return null;
    }
}

