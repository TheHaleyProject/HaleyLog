using Haley.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Haley.Log;

namespace Haley.Models
{
    internal class DetailedTextLogWriter : LogWriterBase
    {
        public DetailedTextLogWriter(string file_location, string file_name) : base(file_location, file_name, "txt") { }

        public override void Write(LogData data)
        {
            string _towrite = (string)Convert(data);
            using (StreamWriter swriter = File.AppendText(outputFilePath))
            {
                swriter.WriteLine(_towrite);
            }
        }
        public override void Write(List<LogData> dataList)
        {
            string _towrite = (string)Convert(dataList);
            using (StreamWriter swriter = File.AppendText(outputFilePath))
            {
                swriter.WriteLine(_towrite);
            }
        }
        public override object Convert(List<LogData> data)
        {
            StringBuilder mainbuilder = new StringBuilder();
            foreach (var item in data)
            {
                //Get the primary values
                mainbuilder.AppendLine((string)Convert(item)); //convert each sinlge entry
                if (item.Children.Count > 1)
                {
                    mainbuilder.AppendLine((string)Convert(item.Children, true));
                }
            }
            return mainbuilder.ToString();
        }
        public override object Convert(LogData data)
        {
            StringBuilder sbuilder = new StringBuilder();
            if (is_sub)
            {
                sbuilder.AppendLine("#####*****---- BEGIN SUB LOG ----*****#####");
            }
            else
            {
                sbuilder.AppendLine("---- BEGIN LOG ----");
            }
            //Get timestamp
            sbuilder.AppendLine(nameof(data.TimeStamp) + " : " + data.TimeStamp.ToString(logTimeFormat));
            //Get PropertyName
            if (!string.IsNullOrEmpty(data.Title)) sbuilder.AppendLine(nameof(data.Title) + " : " + data.Title);
            //Get the main message if present
            if (!string.IsNullOrEmpty(data.Message)) sbuilder.AppendLine(nameof(data.Message) + " : " + data.Message);
            //Get the Info Type
            sbuilder.AppendLine(nameof(data.Loglevel) + " : " + data.Loglevel.ToString());

            //Get Further Data if it is exception type
            if (data.GetType() == typeof(ExceptionLog))
            {
                ExceptionLog _excplog = (ExceptionLog)data;
                if (!string.IsNullOrEmpty(_excplog.ExceptionMessage)) sbuilder.AppendLine(nameof(_excplog.ExceptionMessage) + " : " + _excplog.ExceptionMessage);
                if (!string.IsNullOrEmpty(_excplog.Trace)) sbuilder.AppendLine(nameof(_excplog.Trace) + " : " + _excplog.Trace);
            }

            //Get data if it is property type
            if (data.GetType() == typeof(DictionaryLog))
            {
                DictionaryLog _dicLog = (DictionaryLog)data;
                if (!string.IsNullOrEmpty(_dicLog.Key)) sbuilder.AppendLine(nameof(_dicLog.Key) + " : " + _dicLog.Key);
                if (!string.IsNullOrEmpty(_dicLog.Value)) sbuilder.AppendLine(nameof(_dicLog.Value) + " : " + _dicLog.Value);
            }

            if (is_sub)
            {
                sbuilder.AppendLine("#####*****---- END SUB LOG ----*****#####");
            }
            else
            {
                sbuilder.AppendLine("---- END LOG ----");
            }

            return sbuilder.ToString();
        }
    }
}
