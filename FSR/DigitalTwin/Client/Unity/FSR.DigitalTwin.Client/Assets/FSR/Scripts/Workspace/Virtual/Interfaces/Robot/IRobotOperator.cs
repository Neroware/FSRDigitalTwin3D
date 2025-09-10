namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces.Robot
{

    public interface IRobotOperator
    {
        public bool IsBusy { get; }
        public string RunningOperation { get; }
    }

}