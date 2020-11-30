using StaticProxyInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy.DemoCommon
{
    [StaticProxyGenerate]
    public interface IReplyMethods
    {

        Task ReceiveMessage(string theMessage);

    }
}
