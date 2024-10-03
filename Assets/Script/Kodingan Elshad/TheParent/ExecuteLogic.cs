using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.AI;
using System;

/* PERHATIAN!!!
 * Kalo mau akses logic di skrip ini
 * Public class nya extend ke class ini
 * Jangan ke MonoBehaviour
 */



abstract class slajdkadj : MonoBehaviour
{
    public void BaseShoot()
    {

    }

    public abstract void asdads();
    public virtual void dasdad()
    {

    }
}

public class ExecuteLogic : AILogic
{

    

    //setelah di extend, klean bisa make function ini tanpa perlu refrence

    public void StartingSetup()
    {
        GameManager gm = GameManager.instance;
        gm.playerGameObject[1].GetComponent<FriendsAI>().friendsID = 1;
        gm.playerGameObject[2].GetComponent<FriendsAI>().friendsID = 2;
    }

    //untuk Reloading
    public void Reload(WeaponStatSO weaponStatSO)
    {
        int bulletNeed = weaponStatSO.magSize - weaponStatSO.currBullet;
        if (weaponStatSO.totalBullet >= bulletNeed)
        {
            weaponStatSO.currBullet = weaponStatSO.magSize;
            weaponStatSO.totalBullet -= bulletNeed;
        }
        else if (weaponStatSO.totalBullet > 0)
        {
            weaponStatSO.currBullet += weaponStatSO.totalBullet;
            weaponStatSO.totalBullet = 0;
        }        

        Debug.Log("Reload");
    }

    //untuk ganti weapon
    public void ChangeWeapon(PlayerAction playerAction, WeaponStatSO[] weaponStats, int weaponNum)
    {
        if (weaponNum >= weaponStats.Length - 1)
        {
            weaponNum = 0;
        }
        else
        {
            weaponNum++;
            if (weaponStats[weaponNum] == null)
            {
                weaponNum--;
            }

        }
        Debug.Log(weaponStats[weaponNum].weaponName);
        playerAction.SetCurrentWeapon(weaponStats[weaponNum], weaponNum);
    }

    //logic 'Shoot'
    public void Shoot(Vector3 origin, Vector3 direction, WeaponStatSO weaponStat, LayerMask entityMask)
    {
        WeaponLogicHandler weaponHandler = new WeaponLogicHandler();
        weaponHandler.ShootingPerformed(origin, direction, weaponStat, entityMask);
    }    


    //logic 'SilentKill'
    public void SilentKill()
    {
        SilentKill silentKill = GetComponentInChildren<SilentKill>();
        silentKill.canKill = true;
    }

    //Logic 'Scope'
    public void Scope()
    {
        GameManager gm = GameManager.instance;

        if(!gm.scope)
        {
            gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 40;
            gm.scope = true;
        }
        else
        {
            gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 70;
            gm.scope = false;
        }

    }


    //Logic 'Switch Character'
    public void SwitchCharacter()
    {
        //kategori untuk refrensikan yang diperlukan
        GameManager gm = GameManager.instance;        

        //kategori logic script
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerAction>().enabled = false;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerCamera>().enabled = false;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = true;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<FriendsAI>().enabled = true;

        //kategori kamera
        gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 60;
        gm.followCameras[gm.playableCharacterNum].Priority = 1;        
        gm.scope = false;
        StartCoroutine(CameraDelay(gm));

        //kategori untuk friendsAI
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<FriendsAI>().friendsID = 1;

        gm.playableCharacterNum++;

        if (gm.playableCharacterNum >= gm.playerGameObject.Length) // check if playerNumber more than playableCharacters.Length, then playerNumber back to 0
        {
            gm.playableCharacterNum = 0;
        }

        SetActiveCharacter(gm, gm.playableCharacterNum);

        StartCoroutine(Switching(gm));
    }

    //Logic 'Mengaktifkan karakter ketika di switch'
    private void SetActiveCharacter(GameManager gm, int playerNumber)
    {       
        //kategori logic script
        gm.playerGameObject[playerNumber].GetComponent<PlayerAction>().enabled = true;        
        gm.playerGameObject[playerNumber].GetComponent<PlayerCamera>().enabled = true;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<FriendsAI>().enabled = false;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = false;


        //kategori kamera
        gm.followCameras[playerNumber].Priority = 2;

        //kategori untuk friendsAI
        if(gm.playableCharacterNum == gm.playerGameObject.Length-1)
        {
            gm.playerGameObject[0].GetComponent<FriendsAI>().friendsID = 2;
            gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = false;
        }
        else
        {
            gm.playerGameObject[gm.playableCharacterNum + 1].GetComponent<FriendsAI>().friendsID = 2;
            gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = false;
        }


    }

    //delay untuk switch karakter
    public IEnumerator Switching(GameManager gm)
    {
        gm.canSwitch = false;

        yield return new WaitForSeconds(1);

        gm.canSwitch = true;
    }

    //delay untuk perpindahan kamera
    public IEnumerator CameraDelay(GameManager gm)
    {
        yield return new WaitForSeconds(0.1f);

        gm.playerGameObject[gm.playableCharacterNum].gameObject.transform.GetChild(0).GetChild(0).GetChild(0).eulerAngles = Vector3.zero;
    }

    public IEnumerator ReloadTime(Action<bool> isReloading, float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);

        isReloading(false);
    }

    public IEnumerator FireRate(Action<bool> fireRateOn, float fireRateTime)
    {
        fireRateOn(true);

        yield return new WaitForSeconds(fireRateTime);

        fireRateOn(false);
    }


}
