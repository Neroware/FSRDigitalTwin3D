namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process
{
    public record ProcessResult
    {
        public Process Process { init; get; }
        public object[] InOuts { init; get; }
        public object[] Outputs { init; get; }
        public long TimeStamp { init; get; }
    }

    public record FunctionResult
    {
        public Function Function { init; get; }
        public object[] InOuts { init; get; }
        public object[] Outputs { init; get; }
        public long TimeStamp { init; get; }
    }

}