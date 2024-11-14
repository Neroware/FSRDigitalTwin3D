using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces {
    public interface IDigitalWorkspaceOperational {
        IObservable<ProcessInvocation> ProcessInvoked { get; }
        
        bool RunProcess(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output);
        Task<bool> RunProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output);

        bool LaunchProcess(string ownerId, string processId, IList<object> input, IList<object> inOut);
        Task<bool> LaunchProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut);

        bool GetResult(string ownerId, string processId, IList<object> inOut, IList<object> output);
        Task<bool> GetResultAsync(string ownerId, string processId, IList<object> inOut, IList<object> output);

        void SetResult(ProcessResult result);
        Task SetResultAsync(ProcessResult result);

        bool IsRunning(string ownerId, string processId);
        Task<bool> IsRunningAsync(string ownerId, string processId);

        bool IsCompleted(string ownerId, string processId);
        Task<bool> IsCompletedAsync(string ownerId, string processId);

        public bool HasSucceeded(string ownerId, string processId);
        public Task<bool> HasSucceededAsync(string ownerId, string processId);
    }

}