using System;

namespace FSR.DigitalTwin.Client.Features.DES
{
    // public record HRCProcessResult<T> where T : HRCProcess
    // {
    //     public T Process { init; get; }
    //     public object[] Inputs => Process.Inputs;
    //     public object[] InOuts => Process.InOuts;
    //     public object[] Outputs { init; get; }
    //     public DateTime TimeStamp { init; get; }
    // }

    public record HRCProcessResult
    {
        public virtual HRCProcess Process { init; get; }
        public object[] Inputs => Process.Inputs;
        public object[] InOuts => Process.InOuts;
        public object[] Outputs { init; get; }
        public DateTime TimeStamp { init; get; }
    }

    public record HRCProcessResult<T> : HRCProcessResult where T : HRCProcess
    {
        private T _process;
        public HRCProcessResult(T process)
        {
            _process = process;
        }
        public override HRCProcess Process => _process;
        

        public override HRCProcess Process { get => base.Process; init => base.Process = value; }
    }
}