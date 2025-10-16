// TODO Erbe aus SocialOperatorBase und passe OnFunction(...) an...
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming
{
    public class SkillBasedOperator : SocialOperatorBase
    {
        private readonly Dictionary<Uri, OperatorSkillBase> _skills = new();
        private readonly Dictionary<string, OperatorSkillBase> _shortIds = new();
        private bool _isBusy = false;
        private string _runningOperation = null;

        public override bool IsBusy => _isBusy;
        public override string RunningOperation => _runningOperation;

        private void FindSkills()
        {
            var functions = gameObject.GetComponents<OperatorSkillBase>();
            foreach (var function in functions)
            {
                if (_skills.ContainsKey(function.Id))
                {
                    Debug.LogError($"Duplicate function {function.Id} found in operator {OperatorId}");
                    continue;
                }
                _skills.Add(function.Id, function);
                _shortIds[function.ShortId] = function;
            }
        }

        protected override Task<SkillResult> OnFunction(string function, object[] inputs, object[] inOuts)
        {
            throw new NotImplementedException();
        }

        // protected override async Task<SkillResult> OnFunction(string function, object[] inputs, object[] inOuts)
        // {
        //     // if (_isBusy) throw new InvalidOperationException("Cannot launch function on a busy operator");
        //     // if (_shortIds.TryGetValue(function, out OperatorSkillBase skill))
        //     // {
        //     //     _isBusy = true;
        //     //     await skill.RunAsync(inputs, inOuts, out object[] outputs);
        //     //     _isBusy = false;
        //     //     return new HRCProcessResult<HRCFunction>()
        //     //     {
        //     //         Process = new HRCFunction()
        //     //         {

        //     //         }
        //     //     }
        //     // }
        //     return null;
        // }
    }
}