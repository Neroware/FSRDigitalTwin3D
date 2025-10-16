using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming
{
    public abstract class OperatorSkillBase : MonoBehaviour, IOperatorSkill
    {
        [SerializeField] private string _id;
        [SerializeField] private string _shortId;

        public string ShortId => _shortId;
        public Uri Id => new(_id);

        public virtual SkillResult Run(object[] inputs, object[] inOuts) => RunAsync(inputs, inOuts).Result;
        public abstract Task<SkillResult> RunAsync(object[] inputs, object[] inOuts);
    }
}