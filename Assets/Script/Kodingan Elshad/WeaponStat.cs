using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStat : MonoBehaviour
{
    public string weaponName;               //nama senjatanya
    public weaponType weaponType;           //tipe senjatanya
    public float fireRate;                  //dihitung dalam rounds/sec
    public float baseDamage;                //damage yang diterima di badan
    public float headDamageMultiplier;      //damage yang diterima di kepala
    public float legDamageMultiplier;       //damage yang diterima di kaki
    public float recoil;                    //recoil tembakan nya
    public int range;                       //jarak yang bisa dipakai oleh tembakan nya
    public int magSize;                     //jumlah peluru yang bisa di pakai dalam 1 magazine
    public int magSpare;                    //jumlah magazine yang dibawa
    public bool allowHoldDownButton;        //true jika senjatanya automatic
}
