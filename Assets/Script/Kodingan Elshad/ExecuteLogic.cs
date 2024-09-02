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

    public void SwitchCharacter()
    {
        GameManager gm = GameManager.instance;        

        PlayerAction playerAction = gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerAction>();
        PlayerActionInput inputActions;
        inputActions = playerAction.GetPlayerActionInput();

        gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerAction>().enabled = false;
        gm.followCameras[gm.playableCharacterNum].Priority = 1;

        inputActions.InputPlayerAction.Shooting.Disable();
        inputActions.InputPlayerAction.SilentKill.Disable();
        inputActions.InputPlayerAction.ChangingWeapon.Disable();
        inputActions.InputPlayerAction.ChangePlayer.Disable();

        gm.playableCharacterNum++;

        if (gm.playableCharacterNum >= gm.playerGameObject.Length) // check if playerNumber more than playableCharacters.Length, then playerNumber back to 0
        {
            gm.playableCharacterNum = 0;
        }

        SetActiveCharacter(gm, gm.playableCharacterNum);

        StartCoroutine(Switching(gm));
    }

    private void SetActiveCharacter(GameManager gm, int playerNumber)
    {
        PlayerAction playerAction = gm.playerGameObject[playerNumber].GetComponent<PlayerAction>();
        PlayerActionInput inputActions;
        inputActions = playerAction.GetPlayerActionInput();

        inputActions.InputPlayerAction.Shooting.Enable();
        inputActions.InputPlayerAction.SilentKill.Enable();
        inputActions.InputPlayerAction.ChangingWeapon.Enable();
        inputActions.InputPlayerAction.ChangePlayer.Enable();


        gm.playerGameObject[playerNumber].GetComponent<PlayerAction>().enabled = true;        
        gm.followCameras[playerNumber].Priority = 2;
        
    }

    public IEnumerator Switching(GameManager gm)
    {
        gm.canSwitch = false;

        yield return new WaitForSeconds(1);

        gm.canSwitch = true;
    }
}
