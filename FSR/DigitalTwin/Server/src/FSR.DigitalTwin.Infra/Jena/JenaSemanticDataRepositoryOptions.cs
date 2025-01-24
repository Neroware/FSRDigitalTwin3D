using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Infra.Jena;

public class JenaSemanticDataRepositoryOptions
{
    public const string ConfigurationSection = "JenaDatabaseRepo";
    [Required] public string BaseUrl { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string UserAgent { get; set; } = "";
}
