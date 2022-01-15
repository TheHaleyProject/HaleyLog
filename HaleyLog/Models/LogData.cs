using Haley.Abstractions;
using Haley.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Haley.Models
{
    [XmlRoot("BaseLog")]
    public class LogData : ILogData
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }
        [XmlAttribute("Name")]
        public string Title { get; set; }
        [XmlAttribute("Time")]
        public DateTime TimeStamp { get; set; }
        [XmlAttribute("LogLevel")]
        public LogLevel Loglevel { get; set; }
        [XmlElement(ElementName = "Message")]
        public string Message { get; set; }
        [XmlElement(ElementName = "EventId")]
        public EventId EventId { get; set; }
        [XmlElement(ElementName = "ModuleName")]
        public string ModuleName { get; set; }
        public LogData() { Id = Guid.NewGuid().ToString(); }
    }
}
