using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Models;
using System.ComponentModel;
using System.Collections.Concurrent;
using Haley.Enums;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Timers;


namespace Haley.Abstractions
{
    public interface IProducerConsumerService
    {
        string Id { get; }
        ILogWriter Writer { get; }
        BlockingCollection<LogData> LogItemsQueue { get; }
        void Produce(LogData data);
    }
}
