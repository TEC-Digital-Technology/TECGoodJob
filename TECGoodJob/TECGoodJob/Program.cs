using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Sockets.Core;
using TEC.Core.Sockets.Server;
using TEC.Core.Text.RandomText;
using TECGoodJob.Core;
using TECGoodJob.Core.Info;
using TECGoodJob.Core.Server;

namespace TECGoodJob.JobDispatcher
{
    class Program
    {
        private static SocketServerSettingCollection socketSettingCollection = new SocketServerSettingCollection();
        static void Main(string[] args)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9527);
            WriteInfoToConsole(localEndPoint);
            Mediator mediator = new Mediator();
            Program.socketSettingCollection[SocketServerSettingEnum.BackLog] = 100;
            Program.socketSettingCollection[SocketServerSettingEnum.LocalEndPoint] = localEndPoint;
            Program.socketSettingCollection[SocketServerSettingEnum.MaxConnections] = 30;
            Program.socketSettingCollection[SocketServerSettingEnum.MaxSimultaneousAcceptOperation] = 10;
            Program.socketSettingCollection[SocketServerSettingEnum.MaxProcessingOperationCount] = 3000;
            Program.socketSettingCollection[SocketServerSettingEnum.OperationBufferSize] = 25;
            SocketListener<DataHolder> socketListener = new SocketListener<DataHolder>(mediator, Program.socketSettingCollection, false);
            Program.NodeManager = new NodeManager(socketListener);
            Program.JobMonitor = new JobMonitor(socketListener, Program.NodeManager.ConnectedClientInfo);
            Program.QueueManager = new QueueManager(Program.JobMonitor, Program.NodeManager, socketListener);
            ((INotifyCollectionChanged)Program.JobMonitor.ReadOnlyManagedJobInfo).CollectionChanged += Program.JobMonitor_CollectionChanged;
            Program.NodeManager.ConnectedClientInfo.CollectionChanged += Program.ConnectedClientInfo_CollectionChanged;
            socketListener.inital();
            System.Threading.Thread.Sleep(3000);
            RandomTextGenerator randomTextGenerator = new RandomTextGenerator();
            Program.QueueManager.dispatchJob(new Core.Info.JobInfo()
            {
                JobFunctionKey = "123456",
                JobId = Guid.NewGuid(),
                JobName = $"測試用_{randomTextGenerator.generate(5)}",
                JobStatus = Core.Enums.JobStatus.Pending
            });
            Console.ReadKey();
        }

        private static void JobMonitor_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                (e.NewItems[0] as JobInfo).PropertyChanged += Program.JobInfo_PropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                (e.OldItems[0] as JobInfo).PropertyChanged -= Program.JobInfo_PropertyChanged;

            }
        }

        private static void JobInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            JobInfo jobInfo = sender as JobInfo;
            Console.WriteLine($"作業「{jobInfo.JobName}({jobInfo.JobId})」資料已更新，狀態為:{jobInfo.JobStatus.ToString()}");
        }

        private static void ConnectedClientInfo_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        public static void WriteInfoToConsole(IPEndPoint localEndPoint)
        {
            Console.WriteLine("緩衝區大小 = " + Program.socketSettingCollection[SocketServerSettingEnum.OperationBufferSize].ToString());
            Console.WriteLine("最大客戶端同時連線數量 = " + Program.socketSettingCollection[SocketServerSettingEnum.MaxConnections].ToString());
            Console.WriteLine("最大同時處理傳送/接受作業的連線數量 = " + Program.socketSettingCollection[SocketServerSettingEnum.MaxProcessingOperationCount].ToString());
            Console.WriteLine("等待連接最大上限 = " + Program.socketSettingCollection[SocketServerSettingEnum.BackLog].ToString());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("本機接聽位址 = " + IPAddress.Parse(((IPEndPoint)localEndPoint).Address.ToString()) + ": " + ((IPEndPoint)localEndPoint).Port.ToString());
            Console.WriteLine("伺服器名稱 = " + Environment.MachineName);
            Console.WriteLine();
            Console.WriteLine("請注意防火牆是否已經開啟相對應的埠號");
            Console.WriteLine();
        }
        public static NodeManager NodeManager { set; get; }
        public static JobMonitor JobMonitor { set; get; }
        public static QueueManager QueueManager { set; get; }
    }
}
