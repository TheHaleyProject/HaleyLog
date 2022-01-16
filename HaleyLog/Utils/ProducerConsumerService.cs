using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Security.AccessControl;
using Haley.Abstractions;
using Haley.Models;
using System.ComponentModel;
using System.Collections.Concurrent;
using Haley.Enums;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Timers;
using Trdng =System.Threading;

namespace Haley.Utils
{
    internal class ProducerConsumerService : IProducerConsumerService
    {
        public string Id { get;  }
        public ILogWriter Writer { get; }
        private object consumerLock = new object();
        private object producerLock = new object();
        Timer _consumerMonitor = new Timer(15000) { AutoReset = false }; //Every 8 seconds.
        private Trdng.Thread activeConsumerthread;
        public BlockingCollection<LogData> LogItemsQueue { get; private set; }
        /// <summary>
        /// Produce should call the consume as well
        /// </summary>
        /// <param name="data"></param>
        public void Produce(LogData data)
        {
            //Different loggers will try to add data in this consumer service. 
            LogItemsQueue.Add(data);  //Thread safe adding. Multiple collections can try to add.

            if (activeConsumerthread == null || !activeConsumerthread.IsAlive) //Either null or not alive, then create new thread.
            {
                lock(producerLock)
                {
                    if (activeConsumerthread == null || !activeConsumerthread.IsAlive)
                    {
                        Trdng.Thread newThread = new Trdng.Thread(new Trdng.ThreadStart(() =>
                        {
                            Consume();
                        }));
                        activeConsumerthread = newThread;
                        activeConsumerthread.Start(); //Start the thread.
                    }
                }
            }
            //If there is no new produce for 8 seconds, then we need to ensure that the activeconsumer thread is not running.
            ProcessTimers(); 
        }

        private void Consume()
        {
            try
            {
                //consumerLock is a local logger variable. This lock ensures that same logger doesn't try to access this method through different threads.
                //But, different loggers will have their own method consumption locally.
                lock (consumerLock) //So only one thread enters processing. (for each logger).
                {
                    List<LogData> _data = new List<LogData>();
                    Action writeLog = () =>
                    {
                        List<LogData> _dataCopy = new List<LogData>(_data); //create a copy.
                        _data = new List<LogData>(); //Reset the list.
                        Writer?.Write(_dataCopy);
                    };

                    //TryTake: If the collection is empty, this method immediately returns false.
                    //Take: A call to Take may block until an item is available to be removed.

                    //in our case, we don't need to wait until another item get available because, we are not processing each item per loop, we are trying to process them when they reach a count (say 20 or 30 items). 

                    while (LogItemsQueue.TryTake(out var item, 2000)) //try to take an element with in 2 seconds. If not, then the logdoesn't have an item.
                    {
                        try
                        {
                            //WE WRITE EITHER IF WE REACH 40 ITEMS OR IF WE DONOT RECEIVE ANY DATA FOR 2 SECONDS.
                            //var _item = _logItemQueue.Take(); //Take one item at a time.
                            _data.Add(item);
                            if (_data.Count == 40) //on reaching 40
                            {
                                writeLog.Invoke();
                            }
                        }
                        catch (Exception ex)
                        {
                            //Collection completed.
                        }

                    }
                    if (_data.Count > 0) { writeLog.Invoke(); }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void ProcessTimers()
        {
            //Restart consumer.
            _consumerMonitor.Stop();
            _consumerMonitor.Start();
        }

        private void _consumerMonitorElapsed(object sender, ElapsedEventArgs e)
        {
            //TODO: Check if consumer is working properly or stuck in some kind of error.
            if (LogItemsQueue.Count == 0 && activeConsumerthread != null )
            {
                //After 15 seconds of last log message, we are still active on consumer thread which is not correct.
                //abort active consumer thread.
                activeConsumerthread.Abort(); //VERIFY IF THIS IS REALLY REQUIRED. SOMETIMES, A PROCESS MIGHT TAKE MORE TIME TO COMPLETE. SO NO NEED TO ABORT THE THREAD.
            }
        }

        public ProducerConsumerService(ILogWriter logWriter) 
        {
            Id = Guid.NewGuid().ToString();
            if (logWriter == null)
            {
                throw new ArgumentException("LogWriter cannot be null when settingup a producer consumer service");
            }
            Writer = logWriter;
            LogItemsQueue = new BlockingCollection<LogData>(boundedCapacity: 1500);
            _consumerMonitor.Elapsed += _consumerMonitorElapsed;
        }

    }
}
