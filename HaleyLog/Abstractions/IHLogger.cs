using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Haley.Abstractions
{
    public interface IHLogger :ILogger
    {
        string Name { get; }
        LogLevel AllowedLevel { get; }
        #region Simplified Calls
        void Info(string message,string title = null);
        void Warn(string message, string title = null);
        void Error(string message, string title = null);
        void Critical(string message, string title = null);
        void Exception(Exception exception, string message=null, string title = null);
        void Exception(Exception exception,EventId eventId, string title = null);
        void Trace(string message, string title = null);
        void Trace(Exception exception, string message=null, string title = null);
        void Trace(Exception exception, EventId eventId, string title = null);
        void Trace(string key, string value, string title);
        void Debug(string message, string title = null);
        void Debug(Exception exception,string message =null, string title = null);
        void Debug(Exception exception, EventId eventId, string title = null);
        void Debug(string key, string value, string title);
        void Log(string key, string value, LogLevel log_level, string title = null);
        void Log(string message, Exception exception, LogLevel log_level, string title = null);
        void Log(string message, LogLevel log_level, string title = null);
        #endregion

        #region Main Call
        void Log(string message, Exception exception, LogLevel log_level, EventId eventId, string title = null); //Convert into logdata.
        void Log(ILogData data);
        #endregion
        string GetOutputLocation();
    }
}
