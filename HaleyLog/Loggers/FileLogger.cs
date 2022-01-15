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

namespace Haley.Log
{
    public sealed class FileLogger : LoggerBase
    {
        #region ATTRIBUTES
        private const string SUBLOGKEY = "SUBLOG_PLACEHOLDER";
        private List<LogData> memoryStore;
        private int auto_dump_count;
        private bool should_auto_dump;
        #endregion

        #region Private Build Methods
        private LogData _buildInfo(string message, string prop_name, LogLevel log_level = LogLevel.Information)
        {
            LogData _result = new LogData();
            _result.Title = prop_name ?? string.Empty;
            _result.Message = message;
            _result.Loglevel = log_level;
            _result.TimeStamp = DateTime.UtcNow;
            return _result;
        }

        private LogData _buildException(Exception _exception, string prop_name, string comments)
        {
            ExceptionLog _result = new ExceptionLog();
            _result.Title = prop_name ?? string.Empty;
            _result.Source = _exception.Source;
            _result.Trace = _exception.StackTrace;
            _result.ExceptionMessage = _exception.Message;
            _result.Message = comments ?? string.Empty;
            _result.TimeStamp = DateTime.UtcNow;
            _result.Loglevel = LogLevel.Exception;
            return _result;
        }

        private LogData _buildKVP(string key, string value, string prop_name, string comments)
        {
            DictionaryLog _result = new DictionaryLog();
            _result.Title = prop_name;
            _result.Message = comments ?? string.Empty;
            _result.Key = key;
            _result.Value = value;
            _result.TimeStamp = DateTime.UtcNow;
            _result.Loglevel = LogLevel.Property;
            return _result;
        }
        #endregion

        #region Private Helper Methods
        private void _autoDump()
        {
                if (!should_auto_dump) return; //No further validation required.
                if (_memoryStoreCount(memoryStore.ToList()) > auto_dump_count) DumpMemory();
        }

        private int _memoryStoreCount(List<LogData> source)
        {
            int basecount = source.Count + source.Sum(p=> _memoryStoreCount(p.Children));
            return basecount; //This should give count value of recursive items.
        }

        private void _log (LogData input, bool in_memory, bool is_sub=false)
        {
            if (is_memory_log)
            {
                //irrespective of what the user chooses, if it is a in-memory-log, then always store in memory.
                in_memory = true;
                should_auto_dump = false;
            }

            lock (memoryStore)
            {
                if (in_memory) //Only if we are adding items to memory, we should care about autodumping
                {
                    _autoDump();
                    if (!is_sub)
                    {
                        memoryStore.Add(input); //Storing to the memory
                    }
                    else
                    {
                        //Sub should always be added to last item in memory.
                        LogData last_node;
                        if (memoryStore.Count > 0)
                        {
                            last_node = memoryStore.Last(); //Get last node
                        }
                        else
                        {
                            last_node = _buildInfo("", prop_name: SUBLOGKEY);
                            //add the newly created node to the memory store
                            memoryStore.Add(last_node);
                        }
                        last_node.Children.Add(input);
                    }
                }
                else
                {
                //If it is not in memory, then we should dump whatever in memory irrespective of the count.
                if (memoryStore.Count > 0) DumpMemory();

                _writer.Write(input, is_sub); //writing directly using the writer
                }
            }
     }

        #endregion

        #region Overridden Methods
        /// <summary>
        /// Log the message
        /// </summary>
        /// <param name="message">String value of the message</param>
        /// <param name="log_level">Type of message</param>
        /// <param name="title">Some associated property name</param>
        /// <param name="in_memory">If false, the data is written directly on to the file. If true, the date is stored in memory until dumped.</param>
        /// <returns>GUID value of the log messgae</returns>
        public override string Log(string message, LogLevel log_level = LogLevel.Information, string title = null, bool in_memory = false, bool is_sub = false)
        {
            LogData _infoLog = _buildInfo(message, title, log_level);
            _log(_infoLog, in_memory,is_sub);
            return _infoLog.Id;
        }

        public override string Log(Exception exception, string title = null, bool in_memory = false, bool is_sub = false)
        {
            LogData _exceptionLog = _buildException(exception, title, comments);
            _log(_exceptionLog, in_memory, is_sub);
            return _exceptionLog.Id;
        }

        public override string Log(string key, string value, string title = null, bool in_memory = false, bool is_sub = false)
        {
            LogData _kvpLog = _buildKVP(key, value, title,comments);
            _log(_kvpLog, in_memory, is_sub);
            return _kvpLog.Id;
        }

        /// <summary>
        /// Forced Memory Dump Method which dumps the memorystore data into file and then clears it
        /// </summary>
        public override void DumpMemory() //Should dump into wherever file that we write
        {
            lock (memoryStore)
            {
                if (!is_memory_log) //Write only if it is not a memory log.
                {
                    _writer.Write(memoryStore.ToList());
                }
                ClearMemoryStore();
            }
        }

        #endregion

        #region Public Methods

        public List<ILogData> GetMemoryStore()
        {
            lock(memoryStore)
            {
                return memoryStore.Cast<ILogData>().ToList();
            }
        }

        /// <summary>
        /// This uses the writer and converts the memory store in to its respective format and returns the object
        /// </summary>
        /// <returns></returns>
        public object GetConvertedMemoryStore()
        {
            lock (memoryStore)
            {
            //this should use the converter to convert it to respective object and return it accordingly
            //The consumer sould take the responsibility to cast it accordingly
            return _writer.Convert(memoryStore.ToList());
            }
        }

        public void ClearMemoryStore()
        {
            lock(memoryStore)
            {
                memoryStore.Clear();
            }
        }
        #endregion

        #region Direct Logs
        public string Info(string message, string title = null)
        {
            return Log(message, LogLevel.Information, title);
        }
        public string Warn(string message, string title = null)
        {
            return Log(message, LogLevel.Warning, title);
        }
        public string Error(string message, string title = null)
        {
            return Log(message, LogLevel.Error, title);
        }
        public string Debug(string message, string title = null)
        {
            return Log(message, LogLevel.Debug, title);
        }
        public string Exception(Exception exception, string title = null)
        {
            return Log(exception,comments,title);
        }

        #endregion

        #region memLog
        public string MemLog(string message, LogLevel log_level = LogLevel.Information, string title = null)
        {
            return Log(message, log_level, title, true, false);
        }
        public string MemLog(Exception exception, string title = null)
        {
            return Log(exception, comments, title, true, false);
        }
        public string MemLog(string key, string value, string title = null)
        {
            return Log(key, value, comments, title, true, false);
        }
        #endregion

        #region subLog
        public string SubLog(string message, LogLevel log_level = LogLevel.Information, string title = null)
        {
            return Log(message, log_level, title, false, true);
        }
        public string SubLog(Exception exception, string title = null)
        {
            return Log(exception, comments, title, false, true);
        }
        public string SubLog(string key, string value, string title = null)
        {
            return Log(key, value, comments, title, false, true);
        }
        #endregion

        #region memSubLog
        public string MemSubLog(string message, LogLevel log_level = LogLevel.Information, string title = null)
        {
            return Log(message, log_level, title, true, true);
        }
        public string MemSubLog(Exception exception, string title = null)
        {
            return Log(exception, comments, title, true, true);
        }
        public string MemSubLog(string key, string value, string title = null)
        {
            return Log(key, value, comments, title, true, true);
        }
        #endregion

        #region Initiations
        /// <summary>
        /// Initialization logic for HLOG
        /// </summary>
        /// <param name="output_path">The location where the log file has to be stored</param>
        /// <param name="output_file_name">Name of the file</param>
        /// <param name="_type">File output type</param>
        /// <param name="auto_dump">If set to true, the data in memory is automatically dumped after every <paramref name="max_memory_count"/></param>
        /// <param name="max_memory_count">When memory data reaches this count, data in memory is dumped in to file. Minimum value should be 100 and maximum can be 5000</param>
        public FileLogger(string output_path,string output_file_name, OutputType _type, bool auto_dump = true, int max_memory_count = 100) :base(output_path, output_file_name, _type)
        {
            memoryStore = new List<LogData>();
            should_auto_dump = auto_dump;
            auto_dump_count = (max_memory_count > 100 && max_memory_count < 5000) ? max_memory_count : 500;
        }

        public FileLogger(OutputType _type) : base(_type)
        {
            memoryStore = new List<LogData>();
            should_auto_dump = false;
            auto_dump_count = 0;
        }

        #endregion
    }
}
