using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetAuditor.Models;

public class NetworkDiagnostics
{

    //Check for basic connectivity / internet connection
    public static async Task<bool> CheckPingAsync(string host)
    {
        try
        {
            using var pinger = new Ping();
            var reply = await pinger.SendPingAsync(host, 3000);//timeout by 3 sec
            return reply.Status == IPStatus.Success;

        }
        catch
        {
            return false;
        }
    }

    //DNS check
    public static string GetIpHostName(string host)
    {
        try
        {
            var addresses = Dns.GetHostAddresses(host);
            return addresses.Length > 0 ? addresses[0].ToString() : "No IP was found";
        }
        catch
        {
            return "DNS Lookup failed";
        }
    }

    //check for allowed services through the firewall.
    public static async Task<PortCheckResult> CheckPortAsync(string host, int port, string service)
    {
        var result = new PortCheckResult{Port = port, ServiceName = service, Status = "CLOSED"};

        try
        {
            using var client = new TcpClient();

            var connectTask = client.ConnectAsync(host, port);
            var timeoutTask = Task.Delay(1000);
            var completedTask = await Task.WhenAny(connectTask, timeoutTask);

            if(completedTask == connectTask && client.Connected)
            {
                result.Status = "OPEN";
            }
            else
            {
                result.Status = "TIMEOUT";
            }


        }
        catch
        {
            result.Status = "ERROR";
        }

        return result;
    }
}
