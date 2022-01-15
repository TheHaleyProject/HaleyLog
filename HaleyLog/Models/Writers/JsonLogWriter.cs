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
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using Haley.Utils;

namespace Haley.Models
{
    internal class JSONLogWriter : LogWriterBase
    {
        JsonSerializerOptions _options = null;
        public JSONLogWriter(string file_location, string file_name) : base(file_location, file_name , "json") {
            if (_options == null)
            {
                _options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                };
                _options.Converters.Add(new JsonStringEnumConverter());
                _options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
        }


        private object _convert(object source)
        {
            return source.ToJson();
        }

        public override object Convert(List<LogData> dataList)
        {
            return _convert(dataList);
        }

        public override object Convert(LogData data)
        {
            return _convert(data);
        }


        public override void Write(LogData data)
        {
            List<LogData> _towriteList = new List<LogData>();
            _towriteList.Add(data);
            Write(_towriteList);
        }

        public override void Write(List<LogData> dataList)
        {
            List<LogData> target_list = new List<LogData>();
            //Now try to get the existing file and see if it has any data.
            if (File.Exists(outputFilePath))
            {
                string _parent_json = File.ReadAllText(outputFilePath);
                target_list = JsonSerializer.Deserialize<List<LogData>>(_parent_json, _options);
            }
                target_list.AddRange(dataList);
            string _towrite = (string)Convert(target_list);
            File.WriteAllText(outputFilePath, _towrite);
        }
    }
}
