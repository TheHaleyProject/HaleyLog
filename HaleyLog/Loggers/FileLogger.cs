using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Security.AccessControl;
using Haley.Abstractions;
using Haley.Models;
using System.ComponentModel;
using System.Collections.Concurrent;
using Haley.Enums;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Haley.Log
{
    public sealed class FileLogger : HLoggerBase
    {
        //Each loggerbase will have it's own Producer Consumer Implementation. 
        //The different methods (via different threads) should/could produce and add it to the collection
        //One single thread will consume and then write to the files.
        #region ATTRIBUTES
        OutputType _outputType { get; set; }
        ILogWriter _writer { get; set; }
        string _outputDirectory { get; set; }
        string _fileName { get; set; }
        #endregion

        private BlockingCollection<LogData> _logItemQueue = new BlockingCollection<LogData>(boundedCapacity:500); //can add upto 500 lines of data but it has to be cleared first only then new data can be added.
        private bool isConsuming = false;
        private object consumingObject = new object();

        #region Private Helper Methods
        private bool checkDirectoryAccess()
        {
            try
            {
                //if directory doesn't exist, try to create it. If unable to create, then it means, access is denied.
                if (!Directory.Exists(_outputDirectory)) Directory.CreateDirectory(_outputDirectory);

                var _tempName = Path.GetFileName(Path.GetTempFileName());
                string tempfilepath = Path.Combine(_outputDirectory, _tempName);
                //if directory exists, then we need to check if the user has write access to it.
                using (FileStream fs = File.Create(tempfilepath)) { }
                File.Delete(tempfilepath);
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($@"Log writer doesn't have sufficient rights to write in the directory {_outputDirectory}", ex);
            }
        } //First step to be done.

        private void ConsumeLogs()
        {
            try
            {
                lock (consumingObject) //So only one thread holds it.
                {
                    if (_logItemQueue.Count == 0) return; //Do not proceed as there are not items yet.
                    isConsuming = true;
                    bool _flag = true;
                    while(_flag)
                    {
                        var _data = _logItemQueue.GetConsumingEnumerable().Take(20); //Take 20 items and then write them (if 10 items not available, it will return whatever available below 20.
                        if (_data == null || _data.Count() == 0)
                        {
                            _flag = false;
                            isConsuming = false;
                        }
                        _writer.Write(_data.ToList());
                    }
                }
            }
            catch (Exception)
            {
                //Dont do anything yet.
                isConsuming = false;
            }
        }
        #endregion

        #region Overridden Methods
        public override string GetOutputLocation()
        {
            return _outputDirectory;
        }

        public override void Log(LogData data)
        {
            //Don't write directly using the writer. Use a producer/consumer pattern based implementation.
            //Write all log to a collection. Consumer will then consume them and write using the writer.
            //First come basis
            _logItemQueue.Add(data);  //Thread safe adding. Multiple collections can try to add.
            if (!isConsuming)
            {
                Task.Run(() => ConsumeLogs()); //not asynchronous but on a different thread.
            }
        }
        #endregion

        #region Initiations

        public FileLogger(string name,LogLevel allowedLevel, string outputDirectory,string file_name, OutputType output_type) :base(name ?? "HLogger",allowedLevel)
        {
            _outputType = output_type;

            //First preference.
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                outputDirectory = AppDomain.CurrentDomain?.BaseDirectory; 
            }

            //Second preference
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                var _entryAssembly = Assembly.GetEntryAssembly();
                if (_entryAssembly != null)
                {
                    outputDirectory=Path.GetDirectoryName(_entryAssembly.Location);
                }
            }

            //Last Fall back preference
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                outputDirectory=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HLogs",AppDomain.CurrentDomain?.FriendlyName ?? "ApplicationLogs");
            }

            if (string.IsNullOrWhiteSpace(_fileName))
            {
                _fileName = $@"{AppDomain.CurrentDomain?.FriendlyName}_{Name}_Log";
            }

            _outputDirectory = outputDirectory; //Get directory
            _fileName = _fileName + DateTime.Now.ToString("__yyyy_MM_dd__HH_mm_ss__");
            //Check if the user has proper directory access or throw exception error.
            checkDirectoryAccess();
            _defineLogWriter(); //initiate the writer.
        }
        public FileLogger(string name, LogLevel allowedLevel,OutputType output_type) : this(name, allowedLevel,null,null, output_type) { }

        private void _defineLogWriter()
        {
            _writer = new SimpleTextWriter(_outputDirectory, _fileName);
            switch (_outputType)
            {
                case OutputType.Json:
                    _writer = new JSONLogWriter(_outputDirectory, _fileName);
                    break;
                case OutputType.Xml:
                    _writer = new XMLLogWriter(_outputDirectory, _fileName);
                    break;
                case OutputType.Text_detailed:
                    _writer = new DetailedTextLogWriter(_outputDirectory, _fileName);
                    break;
            }
        }
        
        #endregion
    }
}
