using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class Screw : OperatorSkillBase
    {
        public override Task<SkillResult> RunAsync(object[] inputs, object[] inOuts)
        {
            Task.Delay(1000);
            return Task.FromResult(new SkillResult() { Succeeded = false });
        }
    }
}