using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponGameObjectData
{
    public string weaponName;
    public GameObject gunGameObject;
    public Transform shootPlacement;
    public Transform weaponMagTransform;
    public PlayerGunCollide playerGunCollide;
}
public class WeaponGameObjectDataContainer : MonoBehaviour
{
    [SerializeField] private List<WeaponGameObjectData> _weaponGameObjectDataList;
    private WeaponGameObjectData _currWeaponGameObjectData;

    public Transform GetCurrShootPlacement()
    {
        return _currWeaponGameObjectData.shootPlacement;
    }
    public Transform GetCurrWeaponMagTransform()
    {
        return _currWeaponGameObjectData.weaponMagTransform;
    }
    public PlayerGunCollide GetPlayerGunCollide()
    {
        return _currWeaponGameObjectData.playerGunCollide;
    }

    //Nanti ini diganti berdasarkan nama kalo uda bener
    public void GetCurrWeaponGameObjectData(int idx)
    {
        if(idx >= _weaponGameObjectDataList.Count) return;

        _currWeaponGameObjectData = _weaponGameObjectDataList[idx];
        foreach(WeaponGameObjectData weaponData in _weaponGameObjectDataList)
        {
            if(_currWeaponGameObjectData == weaponData)weaponData.gunGameObject.SetActive(true);
            else weaponData.gunGameObject.SetActive(false);
        }
    }
}

