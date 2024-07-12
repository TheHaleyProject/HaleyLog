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
using System.Timers;
using Haley.Utils;

namespace Haley.Log
{
    public sealed class FileLogger : HLoggerBase
    {
        //If a logger of a certain output (say JSON) is writing to a file, then another logger of output type (say Text) should not be encouraged.
        //if two loggers tri

        private static ConcurrentDictionary<string, IProducerConsumerService> _targetServices = new ConcurrentDictionary<string, IProducerConsumerService>();

        //Each loggerbase will have it's own Producer Consumer Implementation. 
        //The different methods (via different threads) should/could produce and add it to the collection
        //One single thread will consume and then write to the files.
        #region ATTRIBUTES
        OutputType _outputType { get; set; }
        IFileLogWriter _writer { get; set; }
        string _outputDirectory { get; set; }
        string _fileName { get; set; }
        IProducerConsumerService _producerService; //Each target file will have one single producer consumer service.
        #endregion

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
            _producerService?.Produce(data);
        }

        #endregion

        #region Initiations
        private bool ProcessOutputDirectory(FileLoggerOptions options)
        {
            if (options.DirPriority == DirectoryPriority.LocalAppData) {
                //Last Fall back preference
                if (string.IsNullOrWhiteSpace(options.OutputDirectory)) {
                    options.OutputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Haley", AppDomain.CurrentDomain?.FriendlyName ?? "AppLogs");
                }
            }

            //First preference.
            if (string.IsNullOrWhiteSpace(options.OutputDirectory))
            {
                var _entryAssembly = Assembly.GetEntryAssembly();
                if (_entryAssembly != null)
                {
                    options.OutputDirectory = Path.GetDirectoryName(_entryAssembly.Location);
                }
            }

            //Second preference
            if (string.IsNullOrWhiteSpace(options.OutputDirectory))
            {
                options.OutputDirectory = AppDomain.CurrentDomain?.BaseDirectory;
            }

            //Last Fall back preference
            if (string.IsNullOrWhiteSpace(options.OutputDirectory))
            {
                options.OutputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Haley", AppDomain.CurrentDomain?.FriendlyName ?? "AppLogs");
            }

            //Add a subfolder to the outputdirectory
            if (!string.IsNullOrWhiteSpace(options.OutputDirectory))
            {
                options.OutputDirectory = Path.Combine(options.OutputDirectory, "Logs");
            }
            _outputDirectory = options.OutputDirectory; //Get directory

            checkDirectoryAccess();
            if (string.IsNullOrWhiteSpace(_outputDirectory)) return false;
            return true;
        }

        public FileLogger(string name,FileLoggerOptions options) :base(name ?? "HLogger",options.AllowedLogLevel)
        {
            _outputType = options.Type;
            if (!ProcessOutputDirectory(options))
            {
                throw new ArgumentException($@"Unable to process output directory {_outputDirectory}");
            }

            _fileName = options.FileName;
            if (string.IsNullOrWhiteSpace(_fileName))
            {
                _fileName = $@"{AppDomain.CurrentDomain?.FriendlyName ?? "AppLog"}_{DateTime.Now.ToString("yyyy-MM-dd")}"; 
                //If the filename is not provided by default, generate a file name with the friendlyname of the current domain
            }

            _defineLogWriter(); //initiate the writer which should prepare the file path name.

            //FILE NAME IS THE MOST IMPORTANT KEY.
            //FOR EACH UNIQUE FILE, DIFFERENT THREADS CAN PRODUCE LOGS TO SINLGE BLOCKING COLLECTION (MAY COME FROM DIFFERENT THREADS).
            //FOR EACH UNIQUE FILE, USE ONE CONSUMER FOR WRITING (SHOULD BE VIA ONLY ONE THREAD).
            if (!_targetServices.ContainsKey(_writer.OutputFilePath))
            {
                _targetServices.TryAdd(_writer.OutputFilePath, new ProducerConsumerService(_writer));
            }
            _producerService = _targetServices[_writer.OutputFilePath]; //This item is just a reference to the same static item.
        }
       
        private void _defineLogWriter()
        {
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
                case OutputType.Text_simple:
                    _writer = new SimpleTextWriter(_outputDirectory, _fileName);
                    break;
            }
        }
        
        #endregion
    }
}
