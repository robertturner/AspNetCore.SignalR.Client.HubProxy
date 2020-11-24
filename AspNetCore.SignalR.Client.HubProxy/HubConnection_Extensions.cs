using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy
{
    public static class HubConnection_Extensions
    {
        public static T CreateProxy<T>(this HubConnection connection) where T : class
        {
            return (T)new HubProxy(connection, typeof(T)).Proxy;
        }

        public static THub CreateProxy<THub, TClient>(this HubConnection connection, TClient client) 
            where THub : class
            where TClient : class
        {
            return (THub)new HubProxy(connection, typeof(THub), typeof(TClient), client).Proxy;
        }
    }
}
