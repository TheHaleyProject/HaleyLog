using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;

namespace Haley.Abstractions
{
    public interface ILogWriter
    {
        //Logwriters can write to File (FileLogWriter) or console or even directly to some database. But all logwriters will need to implement this interface.
        object Convert(List<LogData> datalist); //To Convert to relevant format
        object Convert(LogData data); //To Convert to relevant format
        void Write(LogData data);
        void Write(List<LogData> datalist);
    }
}
