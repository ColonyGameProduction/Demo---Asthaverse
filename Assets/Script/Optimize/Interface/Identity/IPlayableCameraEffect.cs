using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayableCameraEffect
{
    public bool IsScope{ get;}
    public bool IsNightVision { get;}
    void ScopeCamera(int charaIdx);
    void ResetScope(int charaIdx);
    void NightVision(int charaIdx);
    void ResetNightVision(int charaIdx);

}
