using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Haley.Utils;

namespace Haley.Log
{
    public class FileLogProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return LogStore.GetOrAddFileLogger(categoryName, categoryName);
        }

        public void Dispose()
        {
            //if the logger is used by different injection classes, then disposing will throw off exception. 
            //Investigate with further examples and implement.
        }
    }
}
