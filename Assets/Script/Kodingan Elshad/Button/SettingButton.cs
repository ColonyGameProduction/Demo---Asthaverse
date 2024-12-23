using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingButton : ButtonParent, IPointerEnterHandler, IPointerExitHandler
{
    GameManager gm;
    public GameObject highlightGameObject;

    private void Start()
    {
        gm = GameManager.instance;
    }

    public override void ButtonPressed()
    {
        base.ButtonPressed();
        gm.isAtSetting = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightGameObject.SetActive(true);   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightGameObject.SetActive(false);
    }

}
