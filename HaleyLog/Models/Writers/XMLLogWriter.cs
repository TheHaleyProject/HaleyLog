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
using Haley.Utils;
using System.Xml;

namespace Haley.Models
{
    internal class XMLLogWriter : LogWriterBase
    {
        public XMLLogWriter(string file_location, string file_name) : base(file_location, file_name, "xml") { }

        #region Private Methods
        private const string ROOTNAME = "LoggerBase";
        private XDocument _getXDocument()
        {
            try
            {
                //Try to load the file from filepath. If it is empty, create a new xdocument.
                XDocument xdoc;
                try
                {
                    xdoc = XDocument.Load(outputFilePath);
                }
                catch (Exception)
                {
                    xdoc = new XDocument(new XElement(ROOTNAME));
                }
                return xdoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private XElement _getRoot(XDocument xdoc)
        {
            try
            {
                //Try to get the root and check if it is valid. If it is not valid, create a new xdocument and set the root.
                XElement xroot = xdoc.Root;
                if (xroot.Name != ROOTNAME || xroot == null)
                {
                    xdoc = new XDocument(new XElement(ROOTNAME));
                    xroot = xdoc.Root;
                }
                return xroot;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private object _convert(object source)
        {
            return source.ToXml();
        }
        #endregion

        #region Overridden Methods

        public override object Convert(List<LogData> datalist)
        {
            return _convert(datalist);
        }

        public override object Convert(LogData data)
        {
            return _convert(data);
        }

        public override void Write(LogData data)
        {
            //If sub, read the xml and get the last node and add everything as sub.
            try
            {
                //Get Xdocument and the root element.
                XDocument xdoc = _getXDocument();
                XElement xroot = _getRoot(xdoc);
                XElement input_node = (XElement)Convert(data);
                xroot.Add(input_node); //if not sub, add to the root
                xdoc.Save(outputFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Write(List<LogData> datalist)
        {
            if (datalist.Count == 0) return; //Don't proceed for empty list
            //If sub, read the xml and get the last node and add everything as sub.
            try
            {
                //Get Xdocument and the root element.
                XDocument xdoc = _getXDocument();
                XElement xroot = _getRoot(xdoc);
                var _input_nodes = ((XElement)Convert(datalist)).Elements();
                xroot.Add(_input_nodes); //if not sub, add to the root
                xdoc.Save(outputFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
