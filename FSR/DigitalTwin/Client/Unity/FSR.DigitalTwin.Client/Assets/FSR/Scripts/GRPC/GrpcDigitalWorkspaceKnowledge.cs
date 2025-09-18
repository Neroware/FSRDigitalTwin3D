using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;
using Grpc.Core;

namespace FSR.DigitalTwin.Client.Unity.GRPC
{
    public class GrpcDigitalWorkspaceKnowledge : IDigitalWorkspaceKnowledge
    {
        public IProcessSimulationContext GetContext()
        {
            throw new System.NotImplementedException();
        }

        public GrpcDigitalWorkspaceKnowledge(Channel channel) {
            
        }
    }
}