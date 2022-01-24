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
    public class OutputInfo
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public OutputInfo() { }
    }
}
