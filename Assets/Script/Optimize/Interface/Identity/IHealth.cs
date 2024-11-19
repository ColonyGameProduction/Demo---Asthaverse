using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{

    public armourType GetCharaArmourType{ get; }
    public float TotalHealth{ get;}
    public float CurrHealth{ get;}
    void Hurt(float Damage);
    void Heal(float Healing);
    void Death();
}
