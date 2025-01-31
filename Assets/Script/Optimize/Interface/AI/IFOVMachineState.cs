
public interface IFOVMachineState
{
    public FOVDistState CurrState { get; }
    void FOVStateHandler();
}
