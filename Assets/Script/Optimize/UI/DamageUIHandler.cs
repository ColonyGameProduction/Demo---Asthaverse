using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIHandler : MonoBehaviour
{
    [Header("Damage Visual Phase UI")]
    [SerializeField] private Image  _dmgVisualPhase2;
    [SerializeField] private Image  _dmgVisualPhase3, _dmgVisualPhase4;

    [Header("Damage Direction Indicator UI")]
    [SerializeField] private GameObject _dmgDirContainer;
    [SerializeField] private GameObject _arrowIndicator;
    [SerializeField] private GameObject _upBloodDamage, _rightBloodDamage, _leftBloodDamage, _bottomBloodDamage;

    [ReadOnly(false), SerializeField] private Transform _currPlayable;
    



}
