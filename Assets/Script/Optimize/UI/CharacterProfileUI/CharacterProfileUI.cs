using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfileUI : MonoBehaviour
{
    [ReadOnly(false), SerializeField] private PlayableCharacterIdentity _playableIdentity;

    [Space(1)]
    [Header("UI Container")]
    [SerializeField] private Image _charaFaceImageContainer;
    [SerializeField] private TextMeshProUGUI _charaNameContainer;
    [SerializeField] private Image _weaponImageContainer;
    [SerializeField] private TextMeshProUGUI _weaponNameContainer;
    [SerializeField] private GameObject _HPContainer;
    [SerializeField] private TextMeshProUGUI _currHPContainer;
    [SerializeField] private TextMeshProUGUI _maxHPContainer;
    private RectTransform _weaponImageRectTransform, _charaProfileRectTransform;
    private float _moveWeaponDirX = 200f, _changeWeaponUIAlphaDuration = 0.2f;
    private Vector3 _weaponShowPos;
    private Vector3 _playableHPContainerPos, _friendHPContainerPos, _playableHPContainerScale, _friendHPContainerScale;
    private float  _animationDuration = 0.2f;

    public PlayableCharacterIdentity GetCharaProfileIdentity{get {return _playableIdentity;}}

    private void Awake() 
    {
        _weaponImageRectTransform = _weaponImageContainer.GetComponent<RectTransform>();
    }

    #region Data Function
    public void AssignDataToCharacterProfileUI(PlayableCharacterIdentity playableIdentity)
    {
        _playableIdentity = playableIdentity;
        _playableIdentity.OnPlayableHealthChange += UpdateHealthDataWhenHealthChange;
        _playableIdentity.OnSwitchWeapon += SwitchWeaponUI;

        _charaFaceImageContainer.sprite = playableIdentity.GetCharaStatSO.cropImage;
        _charaNameContainer.text = playableIdentity.GetCharaStatSO.entityName;

        _weaponImageContainer.sprite = _playableIdentity.CurrWeapon.weaponStatSO.gunShilouete;
        _weaponNameContainer.text = _playableIdentity.CurrWeapon.weaponStatSO.weaponName;

        _currHPContainer.text = Mathf.Round(_playableIdentity.CurrHealth).ToString();
        _maxHPContainer.text = _playableIdentity.TotalHealth.ToString();
    }

    public void AssignUIAnimationData(float moveWeaponDirectionX, float changeWeaponUIAlphaDuration, Vector3 WeaponShowPos,  Vector3 playableHPContainerPos, Vector3 friendHPContainerPos, Vector3 playableHPContainerScale, Vector3 friendHPContainerScale, float animationDuration)
    {
        _moveWeaponDirX = moveWeaponDirectionX;
        _changeWeaponUIAlphaDuration = changeWeaponUIAlphaDuration;
        _weaponShowPos = WeaponShowPos;
        // _weaponHidePos = WeaponHidePos;

        _playableHPContainerPos = playableHPContainerPos;
        _friendHPContainerPos = friendHPContainerPos;
        _playableHPContainerScale = playableHPContainerScale;
        _friendHPContainerScale = friendHPContainerScale;
        _animationDuration = animationDuration;
    }
    public void UpdateHealthDataWhenHealthChange(float currHealth, float totalHealth)
    {
        UpdateHealthData();
    }
    public void UpdateHealthData()
    {
        float currHealth = Mathf.Round(_playableIdentity.CurrHealth);
        currHealth = currHealth > 0 ? (!_playableIdentity.IsDead ? currHealth : 0) : 0;
        
        _currHPContainer.text = currHealth.ToString();
        _maxHPContainer.text = _playableIdentity.TotalHealth.ToString();
    }
    private void UpdateWeaponData()
    {
        _weaponImageContainer.sprite = _playableIdentity.CurrWeapon.weaponStatSO.gunShilouete;
        _weaponNameContainer.text = _playableIdentity.CurrWeapon.weaponStatSO.weaponName;
    }
    #endregion
    public void SwitchWeaponUI()
    {
        GameObject weaponImageGameObject = _weaponImageContainer.gameObject;

        // LeanTween.moveX(weaponImageGameObject, weaponImageGameObject.transform.position.x + _moveWeaponDirX, _animationDuration);

        Vector3 newPos = _weaponShowPos;
        newPos.x += _moveWeaponDirX;


        LeanTween.moveLocal(weaponImageGameObject, newPos, _animationDuration);
        LeanTween.alpha(_weaponImageRectTransform, 0, _changeWeaponUIAlphaDuration).setOnComplete(
            ()=>
            {
                UpdateWeaponData();
                // LeanTween.moveX(weaponImageGameObject, weaponImageGameObject.transform.position.x - _moveWeaponDirX, _animationDuration);
                newPos.x -= _moveWeaponDirX;
                LeanTween.moveLocal(weaponImageGameObject, newPos, _animationDuration);
                LeanTween.alpha(_weaponImageRectTransform, 1, _changeWeaponUIAlphaDuration);
            }
        );
    }
    public void ChangeUIComposition(bool isCurrPlayable)
    {
        if(isCurrPlayable)
        {
            LeanTween.moveLocal(_HPContainer, _playableHPContainerPos, _animationDuration);
            LeanTween.scale(_HPContainer, _playableHPContainerScale, _animationDuration);
        }
        else
        {
            LeanTween.moveLocal(_HPContainer, _friendHPContainerPos, _animationDuration);
            LeanTween.scale(_HPContainer, _friendHPContainerScale, _animationDuration);
        }

        _weaponImageContainer.gameObject.SetActive(isCurrPlayable);
        _charaNameContainer.gameObject.SetActive(isCurrPlayable);
    }

    public void SetCharaProfileUIPos(Vector3 newScale, Vector3 newPos)
    {
        Debug.Log(this.gameObject.name + " pindah ke " + newPos + "dengan scale " + newScale);
        LeanTween.scale(this.gameObject, newScale, _animationDuration);
        LeanTween.moveLocal(this.gameObject, newPos, _animationDuration);
    }
    
}
