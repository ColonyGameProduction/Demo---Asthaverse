using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterProfileUIHandler : MonoBehaviour
{
    private PlayableCharacterManager _playableCharacterManager;
    private List<CharacterProfileUI> _charaProfileUIList = new List<CharacterProfileUI>();
    [SerializeField] private GameObject _charaProfileUIPrefab;
    [SerializeField] private GameObject _charaProfileUIContainer;
    [Header("Weapon Chara Profile")] 
    [SerializeField] private float _weaponUIAnimationMoveDirX = 200f;
    [SerializeField] private float _weaponUIAnimationAlphaDuration = 0.2f;
    [SerializeField] private Vector3 _weaponShowPos;

    [Header("Bullet Chara Profile")] 
    [SerializeField] private Vector3 _playableBulletContainerPos;
    [SerializeField] private Vector3 _playableBulletContainerScale;
    [SerializeField] private Vector3 _playableCurrBullContainerPos;
    [SerializeField] private float _playableCurrBullContainerFontSize;
    [SerializeField] private Vector3 _playableTotalBullContainerPos;
    [SerializeField] private Vector3 _friendBulletContainerPos;
    [SerializeField] private Vector3 _friendBulletContainerScale;
    [SerializeField] private Vector3 _friendCurrBullContainerPos;
    [SerializeField] private float _friendCurrBullContainerFontSize;
    [SerializeField] private Vector3 _friendTotalBullContainerPos;

    [Header("Chara Hurt Profile")] 
    [SerializeField] private Vector3 _playableCharaHurtPos;
    [SerializeField] private Vector2 _playableCharaHurtSizeDelta;
    [SerializeField] private Vector3 _friendCharaHurtPos;
    [SerializeField] private Vector2 _friendCharaHurtSizeDelta;

    [Header("Chara Image Profile")]
    [SerializeField] private Vector3 _playableCharaImagePos;
    [SerializeField] private Vector2 _playableCharaImageSizeDelta;
    [SerializeField] private Vector3 _friendCharaImagePos;
    [SerializeField] private Vector2 _friendCharaImageSizeDelta;
    [Header("Chara Name Profile")]
    [SerializeField] private Vector3 _playableCharaNamePos;
    [SerializeField] private float _playableCharaNameFontSize;
    [SerializeField] private Vector3 _friendCharaNamePos;
    [SerializeField] private float _friendCharaImageNameFontSize;

    [Header("Chara Profile")]    
    [SerializeField] private Vector3 _playableProfileScaleSize;
    [SerializeField] private Vector3 _playableProfilePos;
    [SerializeField] private Vector2 _playableProfileSizeDelta;
    [SerializeField] private Vector3 _friendProfileScaleSize, _friendProfilePos, _friendProfileMoveDir;
    [SerializeField] private Vector2 _friendProfileSizeDelta;

    [Space(1)]
    [SerializeField] private float _animationDuration = 0.2f;

    #region Getter Setter Variable
    public PlayableCharacterManager SetPlayableCharaManager {set{_playableCharacterManager = value;}}
    #endregion
    public void AssignCharaProfileUI(PlayableCharacterIdentity playableCharacterIdentity)
    {
        GameObject charaProfileUIGO = Instantiate(_charaProfileUIPrefab, _charaProfileUIContainer.transform);
        CharacterProfileUI characterProfileUI = charaProfileUIGO.GetComponent<CharacterProfileUI>();

        characterProfileUI.AssignDataToCharacterProfileUI(playableCharacterIdentity);
        characterProfileUI.AssignUIAnimationData(_weaponUIAnimationMoveDirX, _weaponUIAnimationAlphaDuration, _weaponShowPos, _playableBulletContainerPos, _friendBulletContainerPos, _playableBulletContainerScale, _friendBulletContainerScale, _animationDuration, _playableCharaHurtPos, _playableCharaHurtSizeDelta, _friendCharaHurtPos, _friendCharaHurtSizeDelta, _playableCharaImagePos, _playableCharaImageSizeDelta, _friendCharaImagePos, _friendCharaImageSizeDelta, _playableCharaNamePos, _playableCharaNameFontSize, _friendCharaNamePos, _friendCharaImageNameFontSize, _playableCurrBullContainerPos, _playableCurrBullContainerFontSize, _playableTotalBullContainerPos, _friendCurrBullContainerPos, _friendCurrBullContainerFontSize, _friendTotalBullContainerPos);

        _charaProfileUIList.Add(characterProfileUI);
    }
    public void SwitchingCharaProfileUI(int currPlayableIdx)
    {
        _charaProfileUIList[currPlayableIdx].ChangeUIComposition(true);
        _charaProfileUIList[currPlayableIdx].SetCharaProfileUIPos(_playableProfileScaleSize, _playableProfilePos, _playableProfileSizeDelta);

        Vector3 friendPos = _friendProfilePos;
        for(int i=1;i<_charaProfileUIList.Count;i++)
        {
            currPlayableIdx+= 1;
            if(currPlayableIdx == _charaProfileUIList.Count)
            {
                currPlayableIdx = 0;
            }
            _charaProfileUIList[currPlayableIdx].ChangeUIComposition(false);
            _charaProfileUIList[currPlayableIdx].SetCharaProfileUIPos(_friendProfileScaleSize, friendPos, _friendProfileSizeDelta);

            friendPos -= _friendProfileMoveDir;
        }
    }
    public void UpdateHealthData(Transform transform)
    {
        foreach(CharacterProfileUI characterProfileUI in _charaProfileUIList)
        {
            characterProfileUI.UpdateHealthDataVisual();
        }
    }
    public void RemoveCharaProfileUI(int idx)
    {
        // Debug.Log("Emang indxnya apa" + idx);
        if(_charaProfileUIList[idx] == null) return;

        CharacterProfileUI deleteProfile = _charaProfileUIList[idx];
        _charaProfileUIList.Remove(deleteProfile);
        Destroy(deleteProfile.gameObject);
    }
}
