using AspNetCore.SignalR.Client.HubProxy.DemoCommon;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy.DemoClient
{
    class ReplyMethods : IReplyMethods
    {
        public Task ReceiveMessage(string theMessage)
        {
            return Task.CompletedTask;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting in 2...");
            await Task.Delay(2000);
            Console.WriteLine("Hello World!");


            var hubConnection = new HubConnectionBuilder()
                .WithUrl("http://127.0.0.1:1984/serverhub")
                .Build();



#if false
            hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                var newMsg = $"Message: {message}";
            });
            var proxy = hubConnection.CreateProxy<IServerHub>();
#else
            var replyMethods = new ReplyMethods();
            var proxy = hubConnection.CreateProxy<IServerHub, IReplyMethods>(replyMethods);
#endif

            await hubConnection.StartAsync();

            var ret1 = await proxy.FudgeAString("first");



        }
    }
}
