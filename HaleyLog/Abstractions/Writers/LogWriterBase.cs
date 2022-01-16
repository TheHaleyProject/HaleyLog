using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using Haley.Abstractions;
using Haley.Models;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Runtime;

namespace Haley.Models
{
    public abstract class LogWriterBase : ILogWriter
    {
        public abstract object Convert(List<LogData> datalist);

        public abstract object Convert(LogData data);

        public abstract void Write(LogData data);

        public abstract void Write(List<LogData> datalist);
    }
}
