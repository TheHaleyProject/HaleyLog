using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;
using Haley.Log.Writers;
using System.IO;
using System.Linq;

namespace Haley.Abstractions
{
    public interface ILogger:ILoggerBase
    {
        List<ILog> GetMemoryStore();
        object GetConvertedMemoryStore();
        void ClearMemoryStore();

        #region Direct Logs
        string Info(string message, string property_name = null);
        string Warn(string message, string property_name = null);
        string Debug(string message, string property_name = null);
        string Error(string message, string property_name = null);
        string Exception(Exception exception, string comments = null, string property_name = null);
        #endregion

        #region memLog
        string MemLog(string message, MessageType msg_type = MessageType.Information, string property_name = null);
        string MemLog(Exception exception, string comments = null, string property_name = null);
        string MemLog(string key, string value, string comments = null, string property_name = null);
        #endregion

        #region subLog
        string SubLog(string message, MessageType msg_type = MessageType.Information, string property_name = null);
        string SubLog(Exception exception, string comments = null, string property_name = null);
        string SubLog(string key, string value, string comments = null, string property_name = null);
        #endregion

        #region memSubLog
        string MemSubLog(string message, MessageType msg_type = MessageType.Information, string property_name = null);
        string MemSubLog(Exception exception, string comments = null, string property_name = null);
        string MemSubLog(string key, string value, string comments = null, string property_name = null);
        #endregion
    }
}
