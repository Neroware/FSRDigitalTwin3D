using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Infra.ROS2;

public class RosWebSocketConnectionOptions
{
    public const string ConfigurationSection = "RosWebSocket";
    [Required] public string BaseUrl { get; set; } = "";
}
