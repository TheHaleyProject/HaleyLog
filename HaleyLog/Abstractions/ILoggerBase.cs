using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;
using Haley.Log.Writers;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Haley.Abstractions
{
    public interface ILoggerBase :ILogger
    {
        #region Simplified Calls
        void Info(string message,string title = null);
        void Warn(string message, string title = null);
        void Error(string message, string title = null);
        void Exception(Exception exception, string title = null);
        void Exception(Exception exception,EventId eventId, string title = null);
        void Critical(string message, string title = null);
        void Trace(string message, string title = null);
        void Trace(Exception exception, string title = null);
        void Trace(Exception exception, EventId eventId, string title = null);
        void Trace(string key, string value, string title);
        void Debug(string message, string title = null);
        void Debug(Exception exception, string title = null);
        void Debug(Exception exception, EventId eventId, string title = null);
        void Debug(string key, string value, string title);
        void Log(Exception exception, string title = null);
        void Log(Exception exception, EventId eventId, string title = null);
        void Log(string key, string value, string title);
        #endregion

        #region Main Call
        void Log(string message, LogLevel log_level,EventId eventId, string title);
        #endregion
        string GetDirectory();
    }
}
