using FSR.DigitalTwin.App.Common.Middleware;
using FSR.DigitalTwin.App.Interfaces.Services;

namespace FSR.DigitalTwin.App.Services;

public class RosRobotService : IRobotControlService
{
    private readonly IRosWorkspace _rosWorkspace;

    public RosRobotService(IRosWorkspace rosWorkspace) {
        _rosWorkspace = rosWorkspace ?? throw new ArgumentNullException(nameof(rosWorkspace));
    }

    public void RunTest()
    {
        _rosWorkspace.RunRosBridgeTest();
    }
}
