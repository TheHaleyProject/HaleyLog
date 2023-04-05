using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Haley.Utils;
using Microsoft.Extensions.Options;
using Haley.Models;

namespace Haley.Log
{
    [ProviderAlias("FileLog")]
    public class FileLogProvider : ILoggerProvider
    {
        public readonly FileLoggerOptions Options;
        public FileLogProvider(IOptions<FileLoggerOptions> options) { Options = options.Value; }
        public FileLogProvider() { }

        public ILogger CreateLogger(string categoryName)
        {
            return LogStore.GetOrAddFileLogger(categoryName, categoryName,Options);
        }

        public void Dispose()
        {
            //if the logger is used by different injection classes, then disposing will throw off exception. 
            //Investigate with further examples and implement.
        }
    }
}
