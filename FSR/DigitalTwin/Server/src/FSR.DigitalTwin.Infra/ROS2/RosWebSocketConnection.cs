using FSR.DigitalTwin.App.Common.Middleware;
using RosSharp.RosBridgeClient;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

namespace FSR.DigitalTwin.Infra.ROS2;

public class RosWebSocketConnection : IRosWorkspace
{
    private static readonly string uri = "ws://localhost:9090"; // TODO Move this to API layer as it belongs there!

    public void RunRosBridgeTest()
    {
        RosSocket rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));

        // Create a message object
        std_msgs.String message = new std_msgs.String
        {
            data = "Hello ROS from .NET!"
        };

        // // Advertise the topic
        string publication_id = rosSocket.Advertise<std_msgs.String>("my_awesome_topic");

        // // Publish the message
        rosSocket.Publish(publication_id, message);

        Console.WriteLine("Published message: " + message.data);

        rosSocket.Close();
    }
}