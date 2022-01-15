using Haley.Enums;
using Haley.Log.Writers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Haley.Utils;

namespace Haley.Abstractions
{
    public abstract class LoggerBase : ILoggerBase
    {
        protected bool is_memory_log { get; }
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

                string tempfilepath = Path.Combine(output_path, "test.tmptest");
                //if directory exists, then we need to check if the user has write access to it.
                using (FileStream fs = File.Create(tempfilepath)) { }
                File.Delete(tempfilepath);
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Log writer doesn't have sufficient rights to write in the directory", ex);
            }
        } //First step to be done.
        #endregion

        #region DebugMethods
        public string Debug(string message, string property_name = null, bool in_memory = false, bool is_sub = false)
        {
            if (Assembly.GetEntryAssembly().IsDebugBuild())
            {
                return Log(message, MessageType.Debug, property_name, in_memory, is_sub);
            }
            else
            {
                return null;
            }
        }
        public string Debug(Exception exception, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false)
        {
            if (Assembly.GetEntryAssembly().IsDebugBuild())
            {
                return Log(exception, comments, property_name, in_memory, is_sub);
            }
            else
            {
                return null;
            }
        }
        public string Debug(string key, string value, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false)
        {
            if (Assembly.GetEntryAssembly().IsDebugBuild())
            {
                return Log(key, value, comments, property_name, in_memory, is_sub);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Main Abstractions
        public abstract string Log(string message, MessageType msg_type = MessageType.Information, string property_name = null, bool in_memory = false, bool is_sub = false);
        public abstract string Log(Exception exception, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false);
        public abstract string Log(string key, string value, string comments = null, string property_name = null, bool in_memory = false, bool is_sub = false);

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
        #endregion
    }
}
