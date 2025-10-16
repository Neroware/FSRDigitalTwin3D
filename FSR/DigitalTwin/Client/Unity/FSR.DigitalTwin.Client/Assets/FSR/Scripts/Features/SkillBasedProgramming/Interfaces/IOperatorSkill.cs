using System;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces
{
    public interface IOperatorSkill
    {
        string ShortId { get; }
        Uri Id { get; }
        SkillResult Run(object[] inputs, object[] inOuts);
        Task<SkillResult> RunAsync(object[] inputs, object[] inOuts);
    }
}

