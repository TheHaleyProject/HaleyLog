using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using Haley.Abstractions;
using Haley.Enums;
using Haley.Models;
using Haley.Utils;

namespace Haley.Log
{
    public sealed class LogStore
    {
        private ConcurrentDictionary<string, ILoggerBase> _loggers = new ConcurrentDictionary<string, ILoggerBase>();

        public ILoggerBase BaseLog { get; set; } //This is the singleton logger to be used.
        private static bool _initiated = false;
        private  static LogStore _singleton;
        public static LogStore Singleton 
        { 
            get 
            {
                if (!_initiated)
                {
                    _initiated = true;// We have initiated the singleton.
                    //Default logger will happen in the base directory.
                    //_singleton.BaseLog = new HLog(); //Setup Hlog as the first log.
                }
                return _singleton; 
            } 
        }

        public static LogStore CreateSingleton(ILoggerBase sourceLog)
        {
            if (!_initiated)
            {
                _singleton.BaseLog = sourceLog;
                _initiated = true;
            }
            return _singleton;
        }
        public ILoggerBase logger(Enum @enum)
        {
            string _key = @enum.getKey();
            return logger(_key);
        }

        public ILoggerBase logger(string key)
        {
                if(_loggers.ContainsKey(key))
                {
                     ILoggerBase _result = null;
                    _loggers.TryGetValue(key, out _result);
                    return _result;
                }
            return null;
        }

        public bool AddLog(ILoggerBase source,Enum @enum)
        {
            return AddLog(source, @enum.getKey());
        }

        public bool AddLog(ILoggerBase source,  string key)
        {
            if (_loggers.ContainsKey(key)) return false;
            return _loggers.TryAdd(key, source);
        }

        public LogStore() { }
        }
    }
