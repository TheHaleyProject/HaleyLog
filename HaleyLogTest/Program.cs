using Haley.Log;
using Haley.Abstractions;
using Haley.Enums;
using Haley.Models;
using Haley.Utils;
using System;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;


namespace HaleyLogTest
{
    class Program
    {
        static Task[] tasks = new Task[4];
        static void Main(string[] args)
        {
            localProcess1();
            localProcess2();
            localProcess3();
            Task.WhenAll(tasks).Wait();
            //Thread.Sleep(2000);

            Console.WriteLine("Write something");
            var _key = Console.ReadKey();
            Thread.Sleep(3000);
        }
        static void localProcess1()
        {
            var _logger = LogStore.GetOrAddFileLogger("Lingam1", "haleytestLog",OutputType.Json,LogLevel.Trace);

            tasks[0] = Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    LogInfo(i, _logger,LogLevel.Debug);
                }
            });
            tasks[1] = Task.Run(() =>
            {
                for (int j = 4000; j < 5000; j++)
                {
                    LogInfo(j, _logger,LogLevel.Warning);
                }
            });
        }
        static void localProcess2()
        {
            var _logger = LogStore.GetOrAddFileLogger("Lingam2", "Main Window",OutputType.Xml);

           tasks[3]=Task.Run(() =>
            {
                for (int i = 2500; i < 3500; i++)
                {
                    LogInfo(i,_logger);
                }
            });
        }

        static void localProcess3()
        {
            var _logger = LogStore.GetOrAddFileLogger("Lingam3", "MainWindow 2",OutputType.Text_detailed, LogLevel.Information);

            tasks[2] = Task.Run(() =>
            {
                for (int i = 4500; i < 5500; i++)
                {
                    LogInfo(i, _logger,LogLevel.Critical);
                }
            });
        }

        static async void LogInfo(int i,IHLogger _logger,LogLevel loglevel = LogLevel.Information)
        {
            var _message = $@"This is an interesting test  {i}";
            _logger.Log(_message, loglevel);
            Console.WriteLine(_message);
        }
    }
}
