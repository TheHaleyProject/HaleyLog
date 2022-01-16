using Haley.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Haley.Log;

namespace Haley.Models
{
    internal class DetailedTextLogWriter : FileLogWriterBase
    {
        public DetailedTextLogWriter(string file_location, string file_name) : base(file_location, file_name, "detailed.txt") { }

        public override void Write(LogData data)
        {
            string _towrite = (string)Convert(data);
            using (StreamWriter swriter = File.AppendText(OutputFilePath))
            {
                swriter.Write(_towrite); //We already define all the lines required. so direclty write it.
            }
        }
        public override void Write(List<LogData> dataList)
        {
            string _towrite = (string)Convert(dataList);
            using (StreamWriter swriter = File.AppendText(OutputFilePath))
            {
                swriter.Write(_towrite);
            }
        }
        public override object Convert(List<LogData> data)
        {
            StringBuilder mainbuilder = new StringBuilder();
            foreach (var item in data)
            {
                mainbuilder.Append((string)Convert(item)); //convert each sinlge entry
            }
            return mainbuilder.ToString();
        }
        public override object Convert(LogData data)
        {
            StringBuilder sbuilder = new StringBuilder();
            sbuilder.AppendLine("-----------------------");
            //Get timestamp
            sbuilder.AppendLine(string.Format("{0,-15} : {1}", nameof(data.TimeStamp), data.TimeStamp.ToString(logTimeFormat)));
            //Get the Info Type
            sbuilder.AppendLine(string.Format("{0,-15} : {1}",nameof(data.Loglevel) , data.Loglevel.ToString()));
            //Get Module Name
            if (!string.IsNullOrEmpty(data.ModuleName)) sbuilder.AppendLine(string.Format("{0,-15} : {1}",nameof(data.ModuleName) , data.ModuleName));
            //Get PropertyName
            if (!string.IsNullOrEmpty(data.Title)) sbuilder.AppendLine(string.Format("{0,-15} : {1}",nameof(data.Title) ,data.Title));
            //Get the main message if present
            if (!string.IsNullOrEmpty(data.Message)) sbuilder.AppendLine(string.Format("{0,-15} : {1}",nameof(data.Message) ,data.Message));
            //Get the eventId data
            if (data.EventId != default(EventId)) sbuilder.AppendLine(string.Format("{0,-15} : {1}", nameof(data.EventId), data.EventId.Id + " | " + data.EventId.Name));
            //Get the Exception
            if (data.Exception != null) sbuilder.AppendLine(string.Format("{0,-15} : {1}", nameof(data.Exception), data.Exception?.ToString()));
            return sbuilder.ToString();
        }
    }
}
