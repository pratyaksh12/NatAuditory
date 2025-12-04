using System;

namespace NetAuditor.Models;

public class PortCheckResult
{
    public int Port{get; set;}
    public string ServiceName{get; set;} = string.Empty;
    public string Status{get;set;} = "UNKNOWN";

}
