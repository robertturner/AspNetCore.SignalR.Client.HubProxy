using AspNetCore.SignalR.Client.HubProxy.DemoCommon;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy.DemoServer
{
    public class ServerHub : Hub<IReplyMethods>, IServerHub
    {
        public ServerHub(HubComms comms) => Comms = comms;

        HubComms Comms { get; }

        public async Task<string> FudgeAString(string sourceString)
        {
            await Task.Delay(1000);
            try
            {
                await Clients.All.ReceiveMessage("ReplyMsg: " + sourceString);
                //await Clients.All.SendAsync("ReceiveMessage", "here we go");
            }
            catch (Exception ex)
            {

            }

            return (sourceString + " fudged!");
        }
    }
}
