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
public class ExecuteLogic : WeaponLogic
{

    //setelah di extend, klean bisa make function ini tanpa perlu refrence

    //logic 'Shoot'
    public void Shoot()
    {
        GameObject weaponType = GetComponentInChildren<WeaponType>().gameObject;
        ExecuteShooting(weaponType);
    }    

    public void ChangingWeapon()
    {
        ChangeWeapon();
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

        PlayerAction playerAction = gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerAction>();
        PlayerActionInput inputActions;
        inputActions = playerAction.GetPlayerActionInput();

        //kategori logic script
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerAction>().enabled = false;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerCamera>().enabled = false;

        //kategori kamera
        gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 60;
        gm.followCameras[gm.playableCharacterNum].Priority = 1;
        gm.scope = false;

        //kategori logic input action
        inputActions.InputPlayerAction.Disable();


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
        //kategori untuk refrensikan yang diperlukan
        PlayerAction playerAction = gm.playerGameObject[playerNumber].GetComponent<PlayerAction>();
        PlayerActionInput inputActions;
        inputActions = playerAction.GetPlayerActionInput();

        //kategori logic input action
        inputActions.InputPlayerAction.Enable();


        //kategori logic script
        gm.playerGameObject[playerNumber].GetComponent<PlayerAction>().enabled = true;        
        gm.playerGameObject[playerNumber].GetComponent<PlayerCamera>().enabled = true; 
        
        //kategori kamera
        gm.followCameras[playerNumber].Priority = 2;        
    }

    public IEnumerator Switching(GameManager gm)
    {
        gm.canSwitch = false;

        yield return new WaitForSeconds(1);

        gm.canSwitch = true;
    }
}
