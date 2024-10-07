using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Juga Tempat ambil data-data universal yg tidak berhubungan dgn si statemachine 
/// </summary>
public class CharacterIdentity : MonoBehaviour
{
    [SerializeField]private bool _isInputPlayer;
    public bool IsInputPlayer { get { return _isInputPlayer; } }


}
