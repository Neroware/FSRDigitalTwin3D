using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core {

    public static class DigitalWorkspace {

        public enum EOperationMode {
            Sleep = 0, Pull = 1, Push = 2
        };

        private static IDigitalWorkspace _workspace = null;
        public static IDigitalWorkspace Instance => _workspace;

        public static void SetWorkspace(IDigitalWorkspace ws) {
            _workspace ??= ws;
        }

    } 

}
