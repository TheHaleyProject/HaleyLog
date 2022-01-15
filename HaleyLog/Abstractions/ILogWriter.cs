using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;

namespace Haley.Abstractions
{
    public interface ILogWriter
    {
        object Convert(List<LogBase> memoryData, bool is_sub = false); //To Convert to relevant format
        object Convert(LogBase data, bool is_sub = false); //To Convert to relevant format
        void Write(LogBase data,bool is_sub=false);
        void Write(List<LogBase> memoryData, bool is_sub = false);
    }
}
