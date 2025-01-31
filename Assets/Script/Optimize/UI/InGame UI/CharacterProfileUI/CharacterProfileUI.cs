
using TMPro;

using UnityEngine;

using UnityEngine.UI;

public class CharacterProfileUI : MonoBehaviour, IUnsubscribeEvent
{
    [ReadOnly(false), SerializeField] private PlayableCharacterIdentity _playableIdentity;
    private WeaponData _currWeapon;

    [Space(1)]
    [Header("UI Container")]
    private RectTransform _characterProfileContainerRectTransform;
    [Header("Chara Profile")]
    [SerializeField] private Image _charaFaceImageContainer;
    private RectTransform _charaFaceImageRectTransform;
    [SerializeField] private TextMeshProUGUI _charaNameContainer;
    [SerializeField] private CanvasGroup _charaHurtContainer;
    private RectTransform _charaHurtContainerRectTransform;
    private Vector3 _playableCharaHurtPos;
    private Vector2 _playableCharaHurtSizeDelta;
    private Vector3 _friendCharaHurtPos;
    private Vector2 _friendCharaHurtSizeDelta;
    private Vector3 _playableCharaImagePos;
    private Vector2 _playableCharaImageSizeDelta;
    private Vector3 _friendCharaImagePos;
    private Vector2 _friendCharaImageSizeDelta;
    private Vector3 _playableCharaNamePos;
    private float _playableCharaNameFontSize;
    private Vector3 _friendCharaNamePos;
    private float _friendCharaImageNameFontSize;

    // [SerializeField] private 
    [Header("Weapon Profile")]
    [SerializeField] private Image _weaponImageContainer;
    [SerializeField] private TextMeshProUGUI _weaponNameContainer;
    [SerializeField] private GameObject _suppressContainer;
    [SerializeField] private GameObject _bulletContainer;
    [SerializeField] private TextMeshProUGUI _currBulletContainer;
    [SerializeField] private TextMeshProUGUI _maxBulletContainer;
    private RectTransform _weaponImageRectTransform;
    private float _moveWeaponDirX = 200f, _changeWeaponUIAlphaDuration = 0.2f;
    private Vector3 _weaponShowPos;
    private Vector3 _playableBulletContainerPos, _friendBulletContainerPos, _playableBulletContainerScale, _friendBulletContainerScale;

    private Vector3 _playableCurrBullContainerPos;
    private float _playableCurrBullContainerFontSize;
    private Vector3 _playableTotalBullContainerPos;
    private Vector3 _friendCurrBullContainerPos;
    private float _friendCurrBullContainerFontSize;
    private Vector3 _friendTotalBullContainerPos;
    private float  _animationDuration = 0.2f;

    public PlayableCharacterIdentity GetCharaProfileIdentity{get {return _playableIdentity;}}

    private void Awake() 
    {
        _characterProfileContainerRectTransform = GetComponent<RectTransform>();
        _weaponImageRectTransform = _weaponImageContainer.GetComponent<RectTransform>();
        _charaHurtContainerRectTransform = _charaHurtContainer.GetComponent<RectTransform>();
        _charaFaceImageRectTransform = _charaFaceImageContainer.GetComponent<RectTransform>();
        _charaHurtContainer.alpha = 0;
    }

    #region Data Function
    public void AssignDataToCharacterProfileUI(PlayableCharacterIdentity playableIdentity)
    {
        _playableIdentity = playableIdentity;
        _playableIdentity.OnPlayableHealthChange += UpdateHealthData;
        
        _playableIdentity.OnSwitchWeapon += SwitchWeaponUI;
        _playableIdentity.OnPlayableBulletChange += UpdateBulletWeaponData;

        _charaFaceImageContainer.sprite = playableIdentity.GetCharaStatSO.cropImage;
        _charaNameContainer.text = playableIdentity.GetCharaStatSO.entityName.ToUpper();

        UpdateWeaponData();
    }

    public void AssignUIAnimationData(float moveWeaponDirectionX, float changeWeaponUIAlphaDuration, Vector3 WeaponShowPos,  Vector3 playableBulletContainerPos, Vector3 friendBulletContainerPos, Vector3 playableBulletContainerScale, Vector3 friendBulletContainerScale, float animationDuration, Vector3 playableCharaHurtPos, Vector3 playableCharaHurtSizeDelta, Vector3 friendCharaHurtPos, Vector3 friendCharaHurtSizeDelta, Vector3 playableCharaImagePos, Vector2 playableCharaImageSizeDelta, Vector3 friendCharaImagePos, Vector2 friendCharaImageSizeDelta, Vector3 playableCharaNamePos, float playableCharaNameFontSize, Vector3 friendCharaNamePos, float friendCharaImageNameFontSize, Vector3 playableCurrBullContainerPos, float playableCurrBullContainerFontSize, Vector3 playableTotalBullContainerPos, Vector3 friendCurrBullContainerPos, float friendCurrBullContainerFontSize, Vector3 friendTotalBullContainerPos)
    {
        _moveWeaponDirX = moveWeaponDirectionX;
        _changeWeaponUIAlphaDuration = changeWeaponUIAlphaDuration;
        _weaponShowPos = WeaponShowPos;
        // _weaponHidePos = WeaponHidePos;

        _playableBulletContainerPos = playableBulletContainerPos;
        _friendBulletContainerPos = friendBulletContainerPos;
        _playableBulletContainerScale = playableBulletContainerScale;
        _friendBulletContainerScale = friendBulletContainerScale;
        _animationDuration = animationDuration;

        _playableCharaHurtPos = playableCharaHurtPos;
        _playableCharaHurtSizeDelta = playableCharaHurtSizeDelta;
        _friendCharaHurtPos = friendCharaHurtPos;
        _friendCharaHurtSizeDelta = friendCharaHurtSizeDelta;

        _playableCharaImagePos = playableCharaImagePos;
        _playableCharaImageSizeDelta = playableCharaImageSizeDelta;
        _friendCharaImagePos = friendCharaImagePos;
        _friendCharaImageSizeDelta = friendCharaImageSizeDelta;

        _playableCharaNamePos = playableCharaNamePos;
        _playableCharaNameFontSize = playableCharaNameFontSize;
        _friendCharaNamePos = friendCharaNamePos;
        _friendCharaImageNameFontSize = friendCharaImageNameFontSize;

        _playableCurrBullContainerPos = playableCurrBullContainerPos;
        _playableCurrBullContainerFontSize = playableCurrBullContainerFontSize;
        _playableTotalBullContainerPos = playableTotalBullContainerPos;
        _friendCurrBullContainerPos = friendCurrBullContainerPos;
        _friendCurrBullContainerFontSize = friendCurrBullContainerFontSize;
        _friendTotalBullContainerPos = friendTotalBullContainerPos;
    }
    private void UpdateHealthData(float currHealth, float totalHealth)
    {
        if(_playableIdentity.IsPlayerInput) return;

        UpdateHealthDataVisual();
    }
    public void UpdateHealthDataVisual()
    {
        float alphaPoint = Mathf.Clamp01(((_playableIdentity.TotalHealth * 3/4) - _playableIdentity.CurrHealth) / (_playableIdentity.TotalHealth * 3/4));
        _charaHurtContainer.alpha = alphaPoint;
    }
    private void UpdateWeaponData()
    {
        _currWeapon = _playableIdentity.CurrWeapon;
        _weaponImageContainer.sprite = _currWeapon.weaponStatSO.gunShilouete;
        _weaponNameContainer.text = _currWeapon.weaponStatSO.weaponName;

        UpdateBulletWeaponData();
    }

    private void UpdateBulletWeaponData()
    {
        _currBulletContainer.text = ((int)_currWeapon.currBullet).ToString("D3");
        _maxBulletContainer.text = ((int)_currWeapon.totalBullet).ToString("D3");
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
            LeanTween.moveLocal(_bulletContainer, _playableBulletContainerPos, _animationDuration);
            LeanTween.scale(_bulletContainer, _playableBulletContainerScale, _animationDuration);

            LeanTween.moveLocal(_charaHurtContainerRectTransform.gameObject, _playableCharaHurtPos, _animationDuration);
            _charaHurtContainerRectTransform.sizeDelta = _playableCharaHurtSizeDelta;

            LeanTween.moveLocal(_charaFaceImageRectTransform.gameObject, _playableCharaImagePos, _animationDuration);
            _charaFaceImageRectTransform.sizeDelta = _playableCharaImageSizeDelta;

            LeanTween.moveLocal(_currBulletContainer.gameObject, _playableCurrBullContainerPos, _animationDuration);
            _currBulletContainer.fontSize = _playableCurrBullContainerFontSize;
            LeanTween.moveLocal(_maxBulletContainer.gameObject, _playableTotalBullContainerPos, _animationDuration);
        }
        else
        {
            LeanTween.moveLocal(_bulletContainer, _friendBulletContainerPos, _animationDuration);
            LeanTween.scale(_bulletContainer, _friendBulletContainerScale, _animationDuration);

            LeanTween.moveLocal(_charaHurtContainerRectTransform.gameObject, _friendCharaHurtPos, _animationDuration);
            _charaHurtContainerRectTransform.sizeDelta = _friendCharaHurtSizeDelta;

            LeanTween.moveLocal(_charaFaceImageRectTransform.gameObject, _friendCharaImagePos, _animationDuration);
            _charaFaceImageRectTransform.sizeDelta = _friendCharaImageSizeDelta;

            LeanTween.moveLocal(_currBulletContainer.gameObject, _friendCurrBullContainerPos, _animationDuration);
            _currBulletContainer.fontSize = _friendCurrBullContainerFontSize;
            LeanTween.moveLocal(_maxBulletContainer.gameObject, _friendTotalBullContainerPos, _animationDuration);
        }

        _weaponImageContainer.gameObject.SetActive(isCurrPlayable);
        // _charaNameContainer.gameObject.SetActive(isCurrPlayable);
        _suppressContainer.SetActive(isCurrPlayable);
    }

    public void SetCharaProfileUIPos(Vector3 newScaleCharaProfile, Vector3 newPosCharaProfile, Vector2 newSizeCharaProfile)
    {
        // Debug.Log(this.gameObject.name + " pindah ke " + newPos + "dengan scale " + newScale);
        LeanTween.scale(this.gameObject, newScaleCharaProfile, _animationDuration);
        LeanTween.moveLocal(this.gameObject, newPosCharaProfile, _animationDuration);
        _characterProfileContainerRectTransform.sizeDelta = newSizeCharaProfile;

    }

    public void UnsubscribeEvent()
    {
        if(_playableIdentity != null)_playableIdentity.OnSwitchWeapon -= SwitchWeaponUI;
        if(_playableIdentity != null)_playableIdentity.OnPlayableBulletChange -= UpdateBulletWeaponData;
    }
}
