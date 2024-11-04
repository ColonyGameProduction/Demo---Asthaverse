using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableSkill : MonoBehaviour
{
    #region GETTER SETTER VARIABLE
    public virtual bool IsSkillOnGoing {get;}
    #endregion
    
    public abstract void UseSkill();

}
