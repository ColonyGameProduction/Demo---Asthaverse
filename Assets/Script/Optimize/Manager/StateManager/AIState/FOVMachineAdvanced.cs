using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class FOVMachineAdvanced : FOVMachine, IFOVMachineState, IHuntPlayable
{
    #region Normal Variable
    [SerializeField] private FOVDistState _currstate;

    [Header("Misc - Advanced")]
    [SerializeField] protected List<Transform> _otherVisibleTargets = new List<Transform>();
    [SerializeField] protected string _enemyCharaTag;
    protected Transform _closestBreadCrumbs;
    [SerializeField] private float _middleDistanceBuffer = 2f;

    

    #endregion
    #region GETTER SETTER VARIABLE
    
    public FOVDistState CurrState { get{return _currstate;} } // enemy ada d mana
    public List<Transform> OtherVisibleTargets {get {return _otherVisibleTargets;} }
    public Transform ClosestBreadCrumbs {get {return _closestBreadCrumbs;}}
    
    #endregion
    
    public bool IsAlreadyAtMiddleSafeDistance()
    {
        if(_closestDistance > viewRadius - (viewRadius / 3) && _closestDistance <= viewRadius - (viewRadius/3) + _middleDistanceBuffer)return true;
        return false;
    }
    #region States Function
    public void FOVStateHandler()
    {
        // Debug.Log("FOV_Is Update First?");
        // float distance;
        // _currDistance = Mathf.Infinity;

        // foreach()s

        GetClosestEnemy();
        

        if(_closestDistance > viewRadius || VisibleTargets.Count == 0)
        {
            _currstate = FOVDistState.none;
        }
        else if (_closestDistance <= _viewRadius && _closestDistance > _viewRadius - (_viewRadius/3))
        {
            _currstate = FOVDistState.far;
        }
        else if(_closestDistance <= _viewRadius - (_viewRadius / 3) && _closestDistance > _viewRadius - (_viewRadius / 3 * 2))
        {
            _currstate = FOVDistState.middle;
        }
        else if(_closestDistance <= _viewRadius - (_viewRadius / 3*2) && _closestDistance >= 0)
        {
            _currstate = FOVDistState.close;
        }
        
    }
    #endregion
    public override void StartFOVMachine()
    {
        _currstate = FOVDistState.far;
        base.StartFOVMachine();
    }
    public override void StopFOVMachine()
    {
        _currstate = FOVDistState.far;
        base.StopFOVMachine();
        OtherVisibleTargets.Clear();
    }
    protected override void FindTargetFunction()
    {
        base.FindTargetFunction();
        SplittingTheObject();
        
    }
    private void SplittingTheObject()
    {
        List<Transform> toRemove = new List<Transform>();
        toRemove.Clear();
        _otherVisibleTargets.Clear();

        foreach (Transform transform in _visibleTargets)
        {
            if (!transform.gameObject.CompareTag(_enemyCharaTag))
            {
                _otherVisibleTargets.Add(transform);
                toRemove.Add(transform);
            }
        }
        foreach (Transform transform in toRemove)
        {
            _visibleTargets.Remove(transform);
        }
    }

    public void GetClosestBreadCrumbs()
    {
        float _tempDistanceEnemy = Mathf.Infinity;
        _closestBreadCrumbs = null;
        if(OtherVisibleTargets.Count > 0)
        {
            foreach(Transform bread in OtherVisibleTargets)
            {
                if(bread.position == transform.position)continue;
                float currDis = Vector3.Distance(transform.position, bread.position);

                if(_tempDistanceEnemy > currDis)
                {
                    _tempDistanceEnemy = currDis;
                    _closestBreadCrumbs = bread;
                }
            }
        }
        _closestDistance = _tempDistanceEnemy;
        if(_closestBreadCrumbs != null)
        {
            _hasToCheckEnemyLastSeenPosition = true;
            _enemyCharalastSeenPosition = _closestBreadCrumbs.position;
        }
        
    }

    public float GetMinimalPlayableStealth()
    {
        float minStealth = Mathf.Infinity;
        foreach(Transform enemy in VisibleTargets)
        {
            CharacterIdentity chara = enemy.GetComponent<CharacterIdentity>();
            // Debug.Log(chara.gameObject);
            if(minStealth > chara.StealthStat)minStealth = chara.StealthStat;
        }
        return minStealth;
    }
    

}

