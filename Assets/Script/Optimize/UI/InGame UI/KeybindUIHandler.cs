using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class KeybindUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    [Header("Keybind General")]
    private KeybindUIType _currType;
    [SerializeField] private Image _keybindImage;

    [SerializeField] private Sprite _generalSprite;
    [SerializeField] private Sprite _pickUpSprite;
    [SerializeField] private Sprite _commandSprite;
    public static Action<KeybindUIType> OnChangeKeybind;

    [Header("Keybind Floating")]
    [SerializeField] private GameObject _silentTakeDownContainer;
    public static Action<bool> OnShowSilentTakeDownKeybind;

    private void Awake() 
    {
        ToggleSilentTakeDownKeybindContainer(false);

        OnChangeKeybind += ChangeKeybindUI;
        ChangeKeybindUI(KeybindUIType.General);

        OnShowSilentTakeDownKeybind += ToggleSilentTakeDownKeybindContainer;
    }

    private void ToggleSilentTakeDownKeybindContainer(bool show)
    {
        _silentTakeDownContainer.SetActive(show);
    }
    
    private void ChangeKeybindUI(KeybindUIType newType)
    {
        if(newType == _currType) return;

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
    }

    public void UnsubscribeEvent()
    {
        OnChangeKeybind -= ChangeKeybindUI;
        OnShowSilentTakeDownKeybind -= ToggleSilentTakeDownKeybindContainer;
    }
}
