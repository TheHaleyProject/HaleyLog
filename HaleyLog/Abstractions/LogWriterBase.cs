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
    internal abstract class LogWriterBase : ILogWriter
    {
        protected string outputDirectory { get; set; }
        protected string outputFilePath { get; set; }
        protected string logTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public LogWriterBase(string _file_location, string _file_name, string _extension)
        {
             outputDirectory = _file_location;
             outputFilePath = Path.Combine(_file_location, _file_name +"."+ _extension);
        }

        public abstract object Convert(List<LogData> datalist);

        public abstract object Convert(LogData data);

        public abstract void Write(LogData data);

        public abstract void Write(List<LogData> datalist);
    }
}
