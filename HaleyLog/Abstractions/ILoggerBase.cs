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
    public interface ILoggerBase
    {
            string Debug(string message, string property_name = null, bool in_memory = false, bool is_sub = false);
            string Debug(Exception exception, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false);
            string Debug(string key, string value, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false);
            string Log(string message, MessageType msg_type = MessageType.Information, string property_name = null, bool in_memory = false, bool is_sub = false);
            string Log(Exception exception, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false);
            string Log(string key, string value, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false);
            void DumpMemory();
            string GetDirectory();
    }
}
