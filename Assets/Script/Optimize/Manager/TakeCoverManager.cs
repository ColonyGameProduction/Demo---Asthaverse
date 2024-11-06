using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TakeCoverManager : MonoBehaviour
{
    public static TakeCoverManager Instance { get;private set;}
    [SerializeField]private AIBehaviourStateMachine[] _takeCoverMachines;
    private void Awake() 
    {
        Instance = this;
        _takeCoverMachines = FindObjectsOfType<AIBehaviourStateMachine>();
    }
    public bool IsTakeCoverPosOccupied(Vector3 checkPos, AIBehaviourStateMachine from)
    {
        foreach(AIBehaviourStateMachine machine in _takeCoverMachines)
        {
            if(machine == from)continue;
            if(machine.IsTakingCover && machine.TakeCoverPosition == checkPos)
            {
                Debug.Log("it's the same man");
                return true;
            }
        }
        return false;
    }

}
