using FSR.DigitalTwin.App.Common.Middleware;
using Microsoft.Extensions.Options;
using RosSharp.RosBridgeClient;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

namespace FSR.DigitalTwin.Infra.ROS2;

public class RosWebSocketConnection : IRosWorkspace
{
    private string _baseUrl;

    public RosWebSocketConnection(IOptions<RosWebSocketConnectionOptions> options) {
        _baseUrl = options.Value.BaseUrl;
    }

    public void RunRosBridgeTest()
    {
        RosSocket rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(_baseUrl));

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