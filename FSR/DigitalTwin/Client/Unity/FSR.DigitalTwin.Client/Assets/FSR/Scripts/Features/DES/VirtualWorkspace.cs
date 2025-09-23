using FSR.DigitalTwin.Client.Features.DES.Interfaces;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public static class VirtualWorkspace {
        private static IVirtualWorkspace _workspace = null;
        public static IVirtualWorkspace Instance => _workspace;

        public static void SetWorkspace(IVirtualWorkspace ws) {
            _workspace ??= ws;
        }

    } 
}