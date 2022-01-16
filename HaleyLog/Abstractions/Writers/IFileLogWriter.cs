using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;

namespace Haley.Abstractions
{
    public interface IFileLogWriter :ILogWriter
    {
        string OutputFilePath { get; }
    }
}
