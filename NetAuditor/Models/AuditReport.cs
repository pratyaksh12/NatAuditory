using System;

namespace NetAuditor.Models;

public class AuditReport
{
    public string TargeHost{get; set;} = string.Empty;
    public DateTime ScanTime{get; set;}
    public bool IsReachable{get;set;}
    public string DnsResolvedIp{get;set;} = "N/A";
    public List<PortCheckResult> PortScanResult{get; set;} = [];
}
