using System;


public interface IReceiveInputFromPlayer
{
    public bool IsPlayerInput{ get; set; }
    event Action<bool> OnIsPlayerInputChange;
}
