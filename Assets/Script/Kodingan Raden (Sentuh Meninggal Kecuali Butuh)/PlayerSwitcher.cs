using UnityEngine;
using System.Collections;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("All Playable Character follow target")]
    public GameObject[] playableCharacters;

    [Header("All Follow Camera for each character's")]
    public GameObject[] followCameras;

    [Header("Change Switch Character Cooldown Time")]
    public int switchCooldownTime = 1;

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
        playableCharacters[playerNumber].gameObject.GetComponent<PlayerCamera>().enabled = false; // turn off PlayerCamera script from previous main character

        followCameras[playerNumber].SetActive(false); // turn off follow camera from previous main character

        playerNumber++; // increase playerNumber by 1

        if (playerNumber >= playableCharacters.Length) // check if playerNumber more than playableCharacters.Length, then playerNumber back to 0
        {
            playerNumber = 0;
        }

        SetActiveCharacter(playerNumber);

        StartCoroutine(SwitchCharacterCooldown());
    }

    private void SetActiveCharacter(int characterIndex) // set a new main character and camera follow
    {


        playableCharacters[characterIndex].gameObject.GetComponent<PlayerMovement>().enabled = true; // turn on PlayerMovement script for the new main character
        playableCharacters[characterIndex].gameObject.GetComponent<PlayerCamera>().enabled = true; // turn on PlayerCamera script for the new main character

        followCameras[characterIndex].SetActive(true); // turn on follow camera for the new main character
    }
}
