using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces {
    public interface IDigitalWorkspaceOperational {
        bool RunProcess(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output);
        Task<bool> RunProcessAsync(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output);

        bool LaunchProcess(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output);
        Task<bool> LaunchProcessAsync(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output);

        bool GetResult(string actorId, string processId);
        Task<bool> GetResultAsync(string actorId, string processId);

        bool IsRunning(string actorId, string processId);
        Task<bool> IsRunningAsync(string actorId, string processId);

        bool IsCompleted(string actorId, string processId);
        Task<bool> IsCompletedAsync(string actorId, string processId);

        public bool HasSucceeded(string actorId, string processId);
        public Task<bool> HasSucceededAsync(string actorId, string processId);
    }

}