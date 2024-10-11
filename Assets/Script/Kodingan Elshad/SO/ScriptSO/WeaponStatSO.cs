using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStatSO", menuName = "ScriptableObject/WeaponStatSO" )]
public class WeaponStatSO : ScriptableObject
{
    public string weaponName;               //nama senjatanya
    public weaponType weaponType;           //tipe senjatanya
    public float fireRate;                  //dihitung dalam rounds/sec
    public float bulletPerTap;              //berapa banyak peluru yang keluar dalam satu kali klick
    public float baseDamage;                //damage yang diterima di badan
    public float headDamageMultiplier;      //damage yang diterima di kepala
    public float legDamageMultiplier;       //damage yang diterima di kaki
    public float recoil;                    //recoil tembakan nya
    public float reloadTime;                //waktu yang diperlukan untuk reload
    public float range;                       //jarak yang bisa dipakai oleh tembakan nya
    public float magSize;                     //jumlah peluru yang bisa di pakai dalam 1 magazine
    public float magSpare;                    //jumlah magazine yang dibawa
    public float totalBullet;                    //jumlah magazine yang dibawa
    public float currBullet;                    //jumlah magazine yang dibawa
    public bool allowHoldDownButton;        //true jika senjatanya automatic
                                            
    public void Initialise()
    {
        totalBullet = magSize * magSpare;
        currBullet = magSize;
    }
}
