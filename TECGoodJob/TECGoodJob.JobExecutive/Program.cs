using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Sockets.Client;
using TEC.Core.Sockets.Core;
using TECGoodJob.Core;
using TECGoodJob.Core.Client.Event.Authenticate;

namespace TECGoodJob.JobExecutive
{
    class Program
    {

        private static SocketClientSettingCollection socketSettingCollection = new SocketClientSettingCollection();
        static void Main(string[] args)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527);
            socketSettingCollection[SocketClientSettingEnum.MaxConnections] = 30;
            socketSettingCollection[SocketClientSettingEnum.MaxSimultaneousConnectOperation] = 10;
            socketSettingCollection[SocketClientSettingEnum.OperationBufferSize] = 25;
            SocketClient<DataHolder> socketClient = new SocketClient<DataHolder>(new Mediator(), Program.socketSettingCollection, false);
            socketClient.OnConnected += Program.SocketClient_OnConnected;
            Program.JobExecutor = new JobExecutor(socketClient);
            socketClient.inital();
            socketClient.connectToServer(remoteEndPoint);
            Console.ReadKey();
        }

        private static void SocketClient_OnConnected(object sender, ConnectedEventArgs e)
        {
            SocketClient<DataHolder> socketClient = sender as SocketClient<DataHolder>;
            socketClient.sendDataAsync(e.DataHoldingUserToken.TokenId,
                System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ClientAuthenticateEvent()
                {
                    MachineName = Environment.MachineName
                })));
        }
        private static JobExecutor JobExecutor { set; get; }
    }
}
