using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data for helping friendAI
public interface IPlayableFriendDataHelper
{
    public int FriendID { get; set; }
    // public GameObject[] GetFriendsNormalPosition{ get; } // Tempat Teman pergi saat kondisi normal tanpa command
    // public 
    void TurnOnOffFriendAI(bool isTurnOn);
}
