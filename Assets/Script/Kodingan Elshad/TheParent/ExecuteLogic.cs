using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

/* PERHATIAN!!!
 * Kalo mau akses logic di skrip ini
 * Public class nya extend ke class ini
 * Jangan ke MonoBehaviour
 */
public class ExecuteLogic : AILogic
{

    //setelah di extend, klean bisa make function ini tanpa perlu refrence

    //logic 'Shoot'
    public void Shoot()
    {
        WeaponType weaponType = GetComponentInChildren<WeaponType>();
        weaponType.Shooting();
    }    

    public void ChangingWeapon()
    {
        
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
            gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 30;
            gm.scope = true;
        }
        else
        {
            gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 60;
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

        //kategori kamera
        gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 60;
        gm.followCameras[gm.playableCharacterNum].Priority = 1;        
        gm.scope = false;
        StartCoroutine(CameraDelay(gm));

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
        
        //kategori kamera
        gm.followCameras[playerNumber].Priority = 2;        
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
}
