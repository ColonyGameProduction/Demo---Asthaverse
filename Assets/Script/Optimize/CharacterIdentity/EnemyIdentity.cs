using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdentity : CharacterIdentity
{
    public override void ReloadWeapon()
    {
        CurrWeapon.currBullet = CurrWeapon.weaponStatSO.magSize;
    }
}
