using System;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.Operation.Interfaces
{
    public interface IOperatorSkill
    {
        string ShortId { get; }
        Uri Id { get; }
        bool Run(object[] inputs, object[] inOuts, out object[] outputs);
        Task<bool> RunAsync(object[] inputs, object[] inOuts, out object[] outputs);
    }
}

