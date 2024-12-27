using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeybindUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    private KeybindUIType _currType;
    [SerializeField] private Image _keybindImage;

    [SerializeField] private Sprite _generalSprite;
    [SerializeField] private Sprite _pickUpSprite;
    [SerializeField] private Sprite _commandSprite;
    public static Action<KeybindUIType> OnChangeKeybind;
    private void Awake() 
    {
        OnChangeKeybind += ChangeKeybindUI;
        ChangeKeybindUI(KeybindUIType.General);
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
    }
}
