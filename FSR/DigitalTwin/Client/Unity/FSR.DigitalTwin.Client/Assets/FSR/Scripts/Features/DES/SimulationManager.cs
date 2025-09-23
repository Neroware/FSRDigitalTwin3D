using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Common.Utils.Semantic;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public class SimulationManager : MonoBehaviour
    {
        private Dictionary<Uri, IProcessSimulation> _scenarios = new();
        private Uri _activeScenario = null;

        Dictionary<Uri, IProcessSimulation> Scenarios => _scenarios;
        public IProcessSimulation ActiveScenario => _scenarios.ContainsKey(_activeScenario) ?
            _scenarios[_activeScenario] : throw new NullReferenceException("missing scenario, did you forget to add it?");

        public IProcessSimulation AddScenario(Uri scenario)
        {
            ProcessSimulation processSimulation = new();
            _scenarios.Add(scenario, processSimulation);
            return processSimulation;
        }
        public IProcessSimulation AddScenario(string scenario) => AddScenario(UriPrefix.PI + scenario);
        public void SetActiveScenario(Uri scenario)
        {
            _activeScenario = scenario;
        }
        public void SetActiveScenario(string scenario) => AddScenario(UriPrefix.PI + scenario);
    }

}