using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskChanger : MonoBehaviour
{
    private int _currIdx = 0;
    [SerializeField] private GameObject _chosenGameObject;
    [SerializeField] private int[] _chosenLayer;
    public void ChangeLayerMask()
    {
        _chosenGameObject.layer = _chosenLayer[_currIdx];
        _currIdx++;
    }
}
