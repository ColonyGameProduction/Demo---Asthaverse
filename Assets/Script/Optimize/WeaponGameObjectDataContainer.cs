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
    public Transform weaponMagParent;
    public Vector3 oriLocalMagPos, oriLocalEulerAngles;
    public PlayerGunCollide playerGunCollide;
}
public class WeaponGameObjectDataContainer : MonoBehaviour
{
    [SerializeField] private List<WeaponGameObjectData> _weaponGameObjectDataList;
    private WeaponGameObjectData _currWeaponGameObjectData;
    private void Awake() 
    {
        for(int i=0; i < _weaponGameObjectDataList.Count; i++)
        {
            _weaponGameObjectDataList[i].playerGunCollide = _weaponGameObjectDataList[i].gunGameObject.GetComponentInChildren<PlayerGunCollide>();
            _weaponGameObjectDataList[i].weaponMagParent = _weaponGameObjectDataList[i].weaponMagTransform.parent;
        }
    }
    private void Start() 
    {
        SaveOriginalLocalPosRot();
    }
    private void SaveOriginalLocalPosRot()
    {
        for(int i=0; i < _weaponGameObjectDataList.Count; i++)
        {
            _weaponGameObjectDataList[i].oriLocalMagPos = _weaponGameObjectDataList[i].weaponMagTransform.localPosition;
            _weaponGameObjectDataList[i].oriLocalEulerAngles = _weaponGameObjectDataList[i].weaponMagTransform.localEulerAngles;
        }
    }
    public Transform GetCurrShootPlacement()
    {
        return _currWeaponGameObjectData.shootPlacement;
    }
    public Transform GetCurrWeaponMagTransform()
    {
        return _currWeaponGameObjectData.weaponMagTransform;
    }
    public Transform GetCurrWeaponMagParent()
    {
        return _currWeaponGameObjectData.weaponMagParent;
    }
    public Vector3 GetCurrWeaponOriLocalPos()
    {
        return _currWeaponGameObjectData.oriLocalMagPos;
    }
    public Vector3 GetCurrWeaponOriEulerAngles()
    {
        return _currWeaponGameObjectData.oriLocalEulerAngles;
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
    public void HideCurrWeapon()
    {
        if(_currWeaponGameObjectData != null) _currWeaponGameObjectData.gunGameObject.SetActive(false);
    }
}

