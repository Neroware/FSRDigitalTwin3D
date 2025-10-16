using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Client.Features.SkillBasedProgramming
{
    public abstract class OperatorSkillBase : MonoBehaviour, IOperatorSkill
    {
        [SerializeField] private string _id;
        [SerializeField] private string _shortId;

        public string ShortId => _shortId;
        public Uri Id => new(_id);

        public virtual bool Run(object[] inputs, object[] inOuts, out object[] outputs)
        {
            bool res = RunAsync(inputs, inOuts, out object[] outputs_).Result;
            outputs = outputs_;
            return res;
        }
        public abstract Task<bool> RunAsync(object[] inputs, object[] inOuts, out object[] outputs);
    }
}