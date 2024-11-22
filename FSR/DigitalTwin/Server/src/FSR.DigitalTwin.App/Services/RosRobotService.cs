using FSR.DigitalTwin.App.Common.Network;
using FSR.DigitalTwin.App.Interfaces.Services;

namespace FSR.DigitalTwin.App.Services;

public class RosRobotService : IRobotControlService
{
    private readonly IRosConnection _rosConnection;

    public RosRobotService(IRosConnection rosConnection) {
        _rosConnection = rosConnection ?? throw new ArgumentNullException(nameof(rosConnection));
    }

    public void RunTest()
    {
        _rosConnection.RunRosBridgeTest();
    }
}
