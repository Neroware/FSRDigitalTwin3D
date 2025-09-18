namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Core
{
    public static class VirtualWorkspace {
        private static IVirtualWorkspace _workspace = null;
        public static IVirtualWorkspace Instance => _workspace;

        public static void SetWorkspace(IVirtualWorkspace ws) {
            _workspace ??= ws;
        }

    } 
}