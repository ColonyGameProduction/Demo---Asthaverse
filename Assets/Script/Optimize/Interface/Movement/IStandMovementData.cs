

public interface IStandMovementData
{
    public float WalkSpeed { get;}
    public float RunSpeed { get;}
    public float CrouchSpeed { get;}
    public bool IsIdle { get; set;}
    public bool IsWalking { get; set;}
    public bool IsRunning { get; set;}
    public bool IsCrouching { get; set;}
}
