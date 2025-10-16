using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Client.Features.SkillBasedProgramming.Function
{
    public class PickAndPlace : OperatorSkillBase
    {
        [SerializeField] private RosMoveitPickAndPlaceController _controller;

        public override Task<bool> RunAsync(object[] inputs, object[] inOuts, out object[] outputs)
        {
            outputs = new object[] { 0 };
            return Task.FromResult(true);
        }
    }

}