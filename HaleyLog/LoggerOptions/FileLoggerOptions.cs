using Haley.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haley.Models
{
    public sealed class FileLoggerOptions
    {
        public string OutputDirectoryName { get; set; }
        public string FileName { get; set; }
        public bool ShouldGenerateEachDay { get; set; }
        public LogLevel AllowedLogLevel { get; set; }
        public OutputType Type { get; set; }
        public FileLoggerOptions() { Type = OutputType.Text_simple;AllowedLogLevel = LogLevel.Information; }
    }
}
