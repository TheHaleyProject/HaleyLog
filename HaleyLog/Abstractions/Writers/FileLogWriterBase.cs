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
    public abstract class FileLogWriterBase : LogWriterBase, IFileLogWriter
    {
        protected string outputDirectory { get; set; }
        public string OutputFilePath { get; }
        protected string logTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public FileLogWriterBase(string _file_location, string _file_name, string _extension)
        {
             outputDirectory = _file_location;
             OutputFilePath = Path.Combine(_file_location, _file_name +"."+ _extension);
        }
    }
}
