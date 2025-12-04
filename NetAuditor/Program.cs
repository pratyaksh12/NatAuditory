/*
Automating the first 5 minutes of troubleshooting a VoIP issue.
*/

using System.Text.Json;
using NetAuditor.Models;

Console.WriteLine("=====================================================");
Console.WriteLine("      ACCESS PREP: VoIP INFRASTRUCTURE AUDITOR       ");
Console.WriteLine("=====================================================");


Console.WriteLine("\n Enter Target Domain\n");
string target = Console.ReadLine()?.Trim() ?? "";

if (string.IsNullOrWhiteSpace(target)) return;

Console.WriteLine($"\n[1] Starting Diagnostics on: {target}...");

//Initialise the report
var report = new AuditReport { TargeHost = target, ScanTime = DateTime.Now };


//Check for DNS
report.DnsResolvedIp = NetworkDiagnostics.GetIpHostName(target);
Console.WriteLine($"   -> DNS Resolved IP: {report.DnsResolvedIp}");

//Check Ping
report.IsReachable = await NetworkDiagnostics.CheckPingAsync(target);
Console.WriteLine($"  Host Reachable?: {report.IsReachable}");

if (!report.IsReachable)
{
    Console.WriteLine("CRITICAL: HOST IS OFFLINE ABORTING PORT SCAN");
    return;
}

var portsToCheck = new Dictionary<int, string>
        {
            {5060, "SIP (VoIP)"},
            {5061, "SIP-TLS (Secure)"},
            {80, "HTTP (Web)"},
            {443, "HTTPS (Secure)"}
        };


var tasks = new List<Task<PortCheckResult>>();
foreach (var item in portsToCheck)
{
    tasks.Add(NetworkDiagnostics.CheckPortAsync(target, item.Key, item.Value));
}

var results = await Task.WhenAll(tasks);
report.PortScanResult = [.. results];

foreach (var item in report.PortScanResult)
{
    string icon = item.Status == "OPEN" ? "✅" : "⛔";
    Console.WriteLine($"   -> Port {item.Port} ({item.ServiceName}): {item.Status} {icon}");
}

string json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
string fileName = $"Audit_{target}_{DateTime.Now:HHmmss}.json";

await File.WriteAllTextAsync(fileName, json);
Console.WriteLine($"\n[3] Audit Complete. Report saved to {fileName}");
