using System;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy.DemoCommon
{
    [StaticProxy]
    public interface IServerHub
    {
        Task<string> FudgeAString(string sourceString);

    }
}
