using System;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public record HRCProcessResult<T> where T : HRCProcess
    {
        public T Process { init; get; }
        public object[] Inputs => Process.Inputs;
        public object[] InOuts => Process.InOuts;
        public object[] Outputs { init; get; }
        public DateTime TimeStamp { init; get; }
    }
}