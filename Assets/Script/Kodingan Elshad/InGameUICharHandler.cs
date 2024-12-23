using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUICharHandler : MonoBehaviour
{
    GameManager gm;
    public PlayerAction player;
    public WeaponStatSO weapon;

    public Sprite charFace;
    public Sprite weaponShilouete;
    public string charName;
    public string weaponName;

    public string currHP;
    public string maxHP;

    private void Start()
    {
        gm = GameManager.instance;
    }


    private void Update()
    {
        currHP = Mathf.Round(player.playerHP).ToString();
        transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = currHP;
    }

    public void AssigningCharUI(GameObject currPlayer)
    {
        player = currPlayer.GetComponent<PlayerAction>();
        weapon = player.activeWeapon;

        charFace = player.character.cropImage;
        charName = player.character.entityName;

        weaponShilouete = weapon.gunShilouete;
        weaponName = weapon.weaponName;

        currHP = Mathf.Round(player.playerHP).ToString();
        maxHP = player.maxPlayerHP.ToString();

        transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().sprite = charFace;
        transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = charName;

        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = weaponShilouete;
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = weaponName;

        transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = currHP;
        transform.GetChild(0).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = maxHP;
    }
    
}
