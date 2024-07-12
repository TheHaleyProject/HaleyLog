﻿using Haley.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haley.Models
{
    public sealed class FileLoggerOptions
    {
        public string OutputDirectory { get; set; }
        public string FileName { get; set; }
        public bool ShouldGenerateEachDay { get; set; }
        public LogLevel AllowedLogLevel { get; set; }= LogLevel.Information;
        public OutputType Type { get; set; } = OutputType.Text_simple;
        public DirectoryPriority DirPriority { get; set; } = DirectoryPriority.LocalAppData;
        public FileLoggerOptions() { }
    }
}
