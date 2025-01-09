using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableSkill : MonoBehaviour
{
    protected GameManager _gm;
    #region GETTER SETTER VARIABLE
    public virtual bool IsSkillOnGoing {get;}
    #endregion
    
    protected virtual void Start() 
    {
        _gm = GameManager.instance;
    }
    public abstract void UseSkill();

}
