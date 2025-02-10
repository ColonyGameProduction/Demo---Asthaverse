using System;
using UnityEngine;
using UnityEngine.UI;

public class KeybindUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    [Header("Keybind General")]
    private KeybindUIType _currType;
    [SerializeField] private Image _keybindImage;

    [SerializeField] private Sprite _generalSprite;
    [SerializeField] private Sprite _pickUpSprite;
    [SerializeField] private Sprite _commandSprite;
    [SerializeField] private Sprite _sniperSprite;
    private KeybindUIType _oldKeybind;
    public static Action<KeybindUIType> OnChangeKeybind;

    [Header("Keybind Floating")]

    [Header("Silent Take Down Keybind")]
    [SerializeField] private GameObject _silentTakeDownContainer;
    public static Action<bool> OnShowSilentTakeDownKeybind;

    [Header("Interact Keybind")]
    [SerializeField] private Image _interactContainer;
    public static Action<InteractObjType> OnShowInteractKeybind;
    public static Action OnHideInteractKeybind;
    [SerializeField] private Sprite _generalInteractSprite, _pickUpInteractSprite, _reviveInteractSprite;

    [Header("Interact Keybind")]
    [SerializeField] private GameObject _takeCoverContainer;
    public static Action<bool> OnShowTakeCoverKeybind;

    private void Awake() 
    {
        ToggleSilentTakeDownKeybindContainer(false);
        ToggleInteractKeybindContainer(false);

        OnChangeKeybind += ChangeKeybindUI;
        ChangeKeybindUI(KeybindUIType.General);

        OnShowSilentTakeDownKeybind += ToggleSilentTakeDownKeybindContainer;

        OnShowInteractKeybind += ShowInteractKeybind;
        OnHideInteractKeybind += HideInteractKeybind;

        OnShowTakeCoverKeybind += ToggleTakeCoverKeybindContainer;
    }

    #region Silent Take Down Keybind
    private void ToggleSilentTakeDownKeybindContainer(bool show)
    {
        _silentTakeDownContainer.SetActive(show);
    }
    #endregion

    #region TakeCover Keybind
    private void ToggleTakeCoverKeybindContainer(bool show)
    {
        _takeCoverContainer.SetActive(show);
    }
    #endregion

    #region  Interact Keybind
    private void ShowInteractKeybind(InteractObjType interactObjType)
    {
        _interactContainer.sprite = interactObjType == InteractObjType.None || interactObjType == InteractObjType.Interact ? _generalInteractSprite : interactObjType == InteractObjType.Pick_up ? _pickUpInteractSprite : _reviveInteractSprite;
        ToggleInteractKeybindContainer(true);
    }
    private void HideInteractKeybind()
    {
        ToggleInteractKeybindContainer(false);
    }
    private void ToggleInteractKeybindContainer(bool show)
    {
        _interactContainer.gameObject.SetActive(show);
    }
    #endregion

    #region General Keybind
    private void ChangeKeybindUI(KeybindUIType newType)
    {
        if(newType == _currType) return;
        if(_currType == KeybindUIType.Sniper && newType != KeybindUIType.Command)
        {
            newType = _oldKeybind;
        }


        _oldKeybind = _currType;
        _currType = newType;
        if(_currType == KeybindUIType.General)
        {
            _keybindImage.sprite = _generalSprite;
        }
        else if(_currType == KeybindUIType.PickUp)
        {
            _keybindImage.sprite = _pickUpSprite;
        }
        else if(_currType == KeybindUIType.Command)
        {
            _keybindImage.sprite = _commandSprite;
        }
        else if(_currType == KeybindUIType.Sniper)
        {
            _keybindImage.sprite = _sniperSprite;
        }
    }
    #endregion
    public void UnsubscribeEvent()
    {
        OnChangeKeybind -= ChangeKeybindUI;
        OnShowSilentTakeDownKeybind -= ToggleSilentTakeDownKeybindContainer;
        OnShowInteractKeybind -= ShowInteractKeybind;
        OnHideInteractKeybind -= HideInteractKeybind;
        OnShowTakeCoverKeybind -= ToggleTakeCoverKeybindContainer;
    }
}
