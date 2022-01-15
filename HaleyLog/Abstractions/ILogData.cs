using Haley.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Haley.Enums;
using Microsoft.Extensions.Logging;

namespace Haley.Abstractions
{
   public interface ILogData
    {
        /// <summary>
        /// Id for this log message.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Specific Name of header for this log message. could even be the class or method name.
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// From which module this Log is coming from (the name of the logger). A single logger will have same module name across.
        /// </summary>
        string ModuleName { get; set; }
        /// <summary>
        /// Specific Event Id for this error message.
        /// </summary>
        EventId EventId { get; set; }
        /// <summary>
        /// Time of this log
        /// </summary>
        DateTime TimeStamp { get; set; }
        /// <summary>
        /// Actual Message.
        /// </summary>
        string Message { get; set; }
        /// <summary>
        /// Message level.
        /// </summary>
        LogLevel Loglevel { get; set; }
    }
}
