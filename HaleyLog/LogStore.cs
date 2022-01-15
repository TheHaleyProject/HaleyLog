using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using Haley.Abstractions;
using Haley.Enums;
using Haley.Models;
using Haley.Utils;
using Microsoft.Extensions.Logging;


namespace Haley.Log
{
    public static class LogStore
    {
        private static ConcurrentDictionary<string, IHLogger> _loggers = new ConcurrentDictionary<string, IHLogger>();

        private static IHLogger defaultLogger = new FileLogger("HLOG", LogLevel.Information, OutputType.Text_simple);
        public static IHLogger Logger => defaultLogger; //This is the singleton logger to be used.

        public static IHLogger CreateLogger(Enum key, IHLogger logger)
        {
            string _key = key.getKey();
            return CreateLogger(_key, logger);
        }

        public static IHLogger CreateLogger(string key, IHLogger logger)
        {
            if (!_loggers.ContainsKey(key))
            {
                _loggers.TryAdd(key, logger);
            }
            return _loggers[key];
        }

        public static IHLogger GetLogger(Enum key)
        {
            return GetLogger(key.getKey());
        }

        public static IHLogger GetLogger(string key)
        {
            if (_loggers.ContainsKey(key)) return _loggers[key];
            return null;
        }

        public static IHLogger GetOrAddLogger(Enum key,string loggerName = "HLog")
        {
            return GetOrAddLogger(key.getKey());
        }
        public static IHLogger GetOrAddLogger(string key, string loggerName = "HLog")
        {
            var _logger = GetLogger(key);
            if (_logger == null)
            {
                _logger = CreateLogger(key, new FileLogger(loggerName ?? "HLog", LogLevel.Information, OutputType.Text_simple));
            }
            return _logger;
        }

        public static void ChangeAllLogLevels(LogLevel logLevel)
        {
            //this changes log level of all the internal loggers.
            foreach (var logger in _loggers)
            {
                if (logger.Value is HLoggerBase hbase)
                {
                    hbase.SetAllowedLevel(logLevel);
                }
            }
        }

        public static void ChangeLoggerLevel(Enum key, LogLevel logLevel)
        {
            ChangeLoggerLevel(key.getKey(), logLevel); 
        }

        public static void ChangeLoggerLevel(string key, LogLevel logLevel)
        {
            if (_loggers.ContainsKey(key))
            {
                if (_loggers[key] is HLoggerBase hlog)
                {
                    hlog.SetAllowedLevel(logLevel);
                }
            }
        }

    }
        
}
