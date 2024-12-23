using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProfileUIHandler : MonoBehaviour
{
    private PlayableCharacterManager _playableCharacterManager;
    private List<CharacterProfileUI> _charaProfileUIList = new List<CharacterProfileUI>();
    [SerializeField] private GameObject _charaProfileUIPrefab;
    [SerializeField] private GameObject _charaProfileUIContainer;
    [Header("Weapon Chara Profile")] 
    [SerializeField]private float _weaponUIAnimationMoveDirX = 200f;
    [SerializeField]private float _weaponUIAnimationAlphaDuration = 0.2f;

    [Header("HP Chara Profile")] 
    [SerializeField]private Vector3 _playableHPContainerPos;
    [SerializeField]private Vector3 _playableHPContainerScale, _friendHPContainerPos , _friendHPContainerScale;

    [Header("Chara Profile")]    
    [SerializeField]private Vector3 _playableProfileScaleSize;
    [SerializeField]private Vector3 _playableProfilePos;
    [SerializeField]private Vector3 _friendProfileScaleSize, _friendProfilePos, _friendProfileMoveDir;

    [Space(1)]
    [SerializeField]private float _animationDuration = 0.2f;

    #region Getter Setter Variable
    public PlayableCharacterManager SetPlayableCharaManager {set{_playableCharacterManager = value;}}
    #endregion
    public void AssignCharaProfileUI(PlayableCharacterIdentity playableCharacterIdentity)
    {
        GameObject charaProfileUIGO = Instantiate(_charaProfileUIPrefab, _charaProfileUIContainer.transform);
        CharacterProfileUI characterProfileUI = charaProfileUIGO.GetComponent<CharacterProfileUI>();

        characterProfileUI.AssignDataToCharacterProfileUI(playableCharacterIdentity);
        characterProfileUI.AssignUIAnimationData(_weaponUIAnimationMoveDirX, _weaponUIAnimationAlphaDuration, _playableHPContainerPos, _friendHPContainerPos, _playableHPContainerScale, _friendHPContainerScale, _animationDuration);

        _charaProfileUIList.Add(characterProfileUI);
    }
    public void SwitchingCharaProfileUI(int currPlayableIdx)
    {
        _charaProfileUIList[currPlayableIdx].ChangeUIComposition(true);
        _charaProfileUIList[currPlayableIdx].SetCharaProfileUIPos(_playableProfileScaleSize, _playableProfilePos);

        Vector3 friendPos = _friendProfilePos;
        for(int i=1;i<_charaProfileUIList.Count;i++)
        {
            currPlayableIdx+= 1;
            if(currPlayableIdx == _charaProfileUIList.Count)
            {
                currPlayableIdx = 0;
            }
            _charaProfileUIList[currPlayableIdx].ChangeUIComposition(false);
            _charaProfileUIList[currPlayableIdx].SetCharaProfileUIPos(_friendProfileScaleSize, friendPos);

            friendPos -= _friendProfileMoveDir;
        }
    }
    public void UpdateHealthData(Transform transform)
    {
        foreach(CharacterProfileUI characterProfileUI in _charaProfileUIList)
        {
            characterProfileUI.UpdateHealthData();
        }
    }
}
