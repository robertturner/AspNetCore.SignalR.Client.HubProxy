using StaticProxyInterfaces;
using System;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy.DemoCommon
{
    [StaticProxyGenerate]
    public interface IServerHub
    {
        Task<string> FudgeAString(string sourceString);

    }
}
