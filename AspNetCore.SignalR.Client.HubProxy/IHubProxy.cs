using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy
{
    public interface IHubProxy : IDisposable
    {
        HubConnection Connection { get; }
        Type HubType { get; }
        object Proxy { get; }
        Type? ClientType { get; }
        object? ClientObject { get; }
    }
}
