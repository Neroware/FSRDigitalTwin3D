using FSR.DigitalTwin.App.Common.Middleware;
using FSR.DigitalTwin.App.Interfaces.Services.Dummy;

namespace FSR.DigitalTwin.App.Services.Dummy;

public class DummyRosService : IDummyRosService
{
    private readonly IRosWorkspace _rosWorkspace;

    public DummyRosService(IRosWorkspace rosWorkspace) {
        _rosWorkspace = rosWorkspace ?? throw new ArgumentNullException(nameof(rosWorkspace));
    }

    public void RunTest()
    {
        _rosWorkspace.RunRosBridgeTest();
    }
}
