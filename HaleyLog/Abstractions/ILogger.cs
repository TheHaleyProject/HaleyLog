using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;
using Haley.Log.Writers;
using System;
using System.IO;
using System.Linq;

namespace Haley.Abstractions
{
    public interface ILogger:ILoggerBase
    {
        List<ILog> getMemoryStore();
        object getConvertedMemoryStore();
        void clearMemoryStore();
    }
}
