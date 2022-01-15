using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;

namespace Haley.Abstractions
{
    public interface ILogWriter
    {
        object Convert(List<LogData> datalist); //To Convert to relevant format
        object Convert(LogData data); //To Convert to relevant format
        void Write(LogData data);
        void Write(List<LogData> datalist);
    }
}
