using Haley.Abstractions;
using Haley.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Haley.Models
{
    [XmlRoot("Exception")]
    public class ExceptionLog : LogBase
    {
        [XmlElement(ElementName = "Source")]
        public string Source { get; set; }
        [XmlElement(ElementName = "Trace")]
        public string Trace { get; set; }
        [XmlElement(ElementName = "Exception")]
        public string ExceptionMessage { get; set; }
    }
}
