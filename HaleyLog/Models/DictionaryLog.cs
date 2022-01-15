using Haley.Abstractions;
using Haley.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Haley.Models
{
    [XmlRoot("Property")]
    public class DictionaryLog : LogBase
    {
        [XmlAttribute(AttributeName = "Key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }
    }
}
