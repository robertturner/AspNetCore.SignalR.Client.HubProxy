using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy.DemoCommon
{
    [StaticProxy]
    public interface IReplyMethods
    {

        Task ReceiveMessage(string theMessage);

    }
}
