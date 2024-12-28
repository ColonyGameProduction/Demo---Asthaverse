using UnityEngine;
using UnityEngine.UI;

public class ChangeKeybindsUI : MonoBehaviour
{
    public Image keybindPic;

    public Sprite generalKeybind;
    public Sprite pickUpKeybind;
    public Sprite commandKeybind;

    public bool isPickUp = false;
    public bool isCommand = false;

    private void Update()
    {
        if (isPickUp)
        {
            keybindPic.sprite = pickUpKeybind;
        }
        else if (isCommand)
        {
            keybindPic.sprite = commandKeybind;
        }
        else
        {
            keybindPic.sprite = generalKeybind;
        }
    }
}
