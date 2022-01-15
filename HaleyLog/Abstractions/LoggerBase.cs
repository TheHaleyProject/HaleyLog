using Haley.Enums;
using Haley.Log.Writers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Haley.Utils;
using Microsoft.Extensions.Logging;

namespace Haley.Abstractions
{
    public abstract class LoggerBase : ILoggerBase
    {
        //Each loggerbase will have it's own Producer Consumer Implementation. 
        //The different methods (via different threads) should/could produce and add it to the collection
        //One single thread will consume and then write to the files.
        protected string output_path { get; set; }
        protected string output_file_name { get; set; }
        private OutputType output_type { get; set; }
        protected ILogWriter _writer { get; set; }

        #region Internal Methods
        private bool checkDirectoryAccess()
        {
            try
            {
                //if directory doesn't exist, try to create it. If unable to create, then it means, access is denied.
                if (!Directory.Exists(output_path)) Directory.CreateDirectory(output_path);

                var _tempName = Path.GetFileName(Path.GetTempFileName());
                string tempfilepath = Path.Combine(output_path, _tempName);
                //if directory exists, then we need to check if the user has write access to it.
                using (FileStream fs = File.Create(tempfilepath)) { }
                File.Delete(tempfilepath);
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($@"Log writer doesn't have sufficient rights to write in the directory {output_path}", ex);
            }
        } //First step to be done.
        #endregion

        #region DebugMethods
        public string Debug(string message, string title = null)
        {
            return Log(message, LogLevel.Debug, title);
        }
        public string Debug(Exception exception, string title = null )
        {
            return Log(exception, title);
        }
        public string Debug(string key, string value, string title = null)
        {
            return Log(key, value, title);
        }
        #endregion

        #region Main Abstractions
        public abstract string Log(string message, LogLevel log_level = LogLevel.Information, string title = null);
        public abstract string Log(Exception exception, string title = null);
        public abstract string Log(string key, string value, string title = null);

        public abstract void DumpMemory();

        public string GetDirectory()
        {
            if (is_memory_log) return null;
            return output_path;
        }

        #endregion

        #region Initiations
        public LoggerBase(string _output_path, string _output_file_name, OutputType _output_type)
        {
            output_path = _output_path;
            output_file_name = _output_file_name;
            output_type = _output_type;
            is_memory_log = false;

            //Check if the user has proper directory access or throw exception error.
            if (!checkDirectoryAccess()) throw new ArgumentException($@"HLog doesn't have sufficient access rights to the path {Path.GetDirectoryName(_output_path)}");

            //Based on the output type, define the logwriter.
            _defineLogWriter();
        }

        public LoggerBase(OutputType _output_type)
        {
            output_type = _output_type;
            output_path = null;
            output_file_name = null;
            is_memory_log = true;

            //Based on the output type, define the logwriter.
            _defineLogWriter();
        }
        private void _defineLogWriter()
        {
            _writer = new SimpleTextWriter(output_path, output_file_name);
            switch (output_type)
            {
                case OutputType.Json:
                    _writer = new JSONLogWriter(output_path, output_file_name);
                    break;
                case OutputType.Xml:
                    _writer = new XMLLogWriter(output_path, output_file_name);
                    break;
                case OutputType.Text_detailed:
                    _writer = new DetailedTextLogWriter(output_path, output_file_name);
                    break;
            }
        }

        public string Trace(string message, string title = null)
        {
            throw new NotImplementedException();
        }

        public string Trace(Exception exception, string title = null)
        {
            throw new NotImplementedException();
        }

        public string Trace(string key, string value, string title = null)
        {
            throw new NotImplementedException();
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public void Info(string message, string title = null)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message, string title = null)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, string title = null)
        {
            throw new NotImplementedException();
        }

        public void Exception(Exception exception, string title = null)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Trace(string message, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Trace(Exception exception, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Trace(string key, string value, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Debug(string message, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Debug(Exception exception, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Debug(string key, string value, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Log(string message, LogLevel log_level, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Log(Exception exception, string title)
        {
            throw new NotImplementedException();
        }

        void ILoggerBase.Log(string key, string value, string title)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
