using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public float TotalHealth{ get;}
    public float HealthNow{ get;}
    void Hurt(float Damage);
    void Heal(float Healing);
    void Death();
}
