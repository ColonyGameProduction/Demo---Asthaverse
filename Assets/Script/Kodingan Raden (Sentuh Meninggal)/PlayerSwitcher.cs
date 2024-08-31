using UnityEngine;
using Cinemachine;
using System.Collections;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("All Playable Character follow target")]
    public GameObject[] playableCharacters;

    [Header("Change Switch Character Cooldown Time")]
    public int switchCooldownTime = 1;

    [Header("Reference to Follow Camera")]
    public CinemachineVirtualCamera followCamera;

    private int playerNumber = 0;

    private bool canSwitch = true;

    private void Start()
    {
        SetActiveCharacter(playerNumber); // set player one to main character on start
    }

    private void Update()
    {
        // if player press tab on the their keyboard, player can switch playable character
        if (Input.GetKeyDown(KeyCode.Tab) && canSwitch)
        {
            SwitchCharacter();
        }
    }

    private IEnumerator SwitchCharacterCooldown() // give player cooldown for switching character
    {
        canSwitch = false;

        yield return new WaitForSeconds(switchCooldownTime);

        canSwitch = true;
    }

    private void SwitchCharacter() // switch a main character role to another character
    {
        playableCharacters[playerNumber].gameObject.GetComponent<PlayerMovement>().enabled = false; // turn off PlayerMovement script from previous main character

        playerNumber = (playerNumber + 1) % playableCharacters.Length; // switch playerNumber and make sure playerNumber not out of bounds

        SetActiveCharacter(playerNumber);

        StartCoroutine(SwitchCharacterCooldown());
    }

    private void SetActiveCharacter(int characterIndex) // set a new main character and camera follow
    {
        playableCharacters[characterIndex].gameObject.GetComponent<PlayerMovement>().enabled = true; // turn on PlayerMovement script for the new main character

        followCamera.Follow = playableCharacters[characterIndex].transform.GetChild(0); // change cinemachine virtual camera follow component to active playable character
    }
}
