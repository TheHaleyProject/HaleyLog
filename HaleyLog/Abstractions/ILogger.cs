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
        List<ILog> getMemoryStore();
        object getConvertedMemoryStore();
        void clearMemoryStore();

        #region Direct Logs
        string Info(string message, string property_name = null);
        string Warn(string message, string property_name = null);
        string Debug(string message, string property_name = null);

        #endregion

        #region memLog
        string memLog(string message, MessageType msg_type = MessageType.information, string property_name = null);
        string memLog(Exception exception, string comments = null, string property_name = null);
        string memLog(string key, string value, string comments = null, string property_name = null);
        #endregion

        #region subLog
        string subLog(string message, MessageType msg_type = MessageType.information, string property_name = null);
        string subLog(Exception exception, string comments = null, string property_name = null);
        string subLog(string key, string value, string comments = null, string property_name = null);
        #endregion

        #region memSubLog
        string memSubLog(string message, MessageType msg_type = MessageType.information, string property_name = null);
        string memSubLog(Exception exception, string comments = null, string property_name = null);
        string memSubLog(string key, string value, string comments = null, string property_name = null);
        #endregion
    }
}
