using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using Haley.Abstractions;
using Haley.Models;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Haley.Models
{
    internal class SimpleTextWriter :LogWriterBase
    {
        public SimpleTextWriter(string file_location, string file_name) : base(file_location, file_name , "txt") { }

        public override void Write(LogData data)
        {
            string _towrite = (string)Convert(data);
            using (StreamWriter swriter = File.AppendText(outputFilePath))
            {
               swriter.WriteLine(_towrite);
            }
        }
        public override void Write(List<LogData> datalist)
        {
            string _towrite = (string)Convert(datalist);
            using (StreamWriter swriter = File.AppendText(outputFilePath))
            {
               swriter.WriteLine(_towrite);
            }
        }
        public override object Convert(List<LogData> datalist)
        {
            StringBuilder mainbuilder = new StringBuilder();
            foreach (var item in datalist)
            {
                //Get the primary values
                mainbuilder.AppendLine((string) Convert(item)); //convert each sinlge entry
            }
            return mainbuilder.ToString();
        }
        public override object Convert(LogData data)
        {
            StringBuilder sbuilder = new StringBuilder();
            sbuilder.Append(string.Format("{0,-20}", data.TimeStamp.ToString(logTimeFormat)));
            sbuilder.Append(" | ");
            sbuilder.Append(string.Format("{0,-7}", data.Loglevel.ToString().ToUpper()));
            sbuilder.Append(" | ");
            //If Property name is not null, provide it.
            if (!string.IsNullOrEmpty(data.ModuleName))
            {
                sbuilder.Append(data.ModuleName);
                sbuilder.Append(" | ");
            }
            //If Property name is not null, provide it.
            if (!string.IsNullOrEmpty(data.Title))
            {
                sbuilder.Append(data.Title);
                sbuilder.Append(" | ");
            }
            //Add message
            if (!string.IsNullOrEmpty(data.Message))
            {
                sbuilder.Append(data.Message);
                sbuilder.Append(" | ");
            }
            //Add Event Id if available
            if (data.EventId != default(EventId))
            {
                sbuilder.AppendLine(string.Format("{0,26} | {1,-8} | {2}", "| Id -", data.EventId.Id,data.EventId.Name));
            }
            //Add exception
            if (data.Exception != null)
            {
                sbuilder.AppendLine(string.Format("{0,26} | {1}","",data.Exception.ToString()));
            }
           
            return sbuilder.ToString();
        }
    }
}
