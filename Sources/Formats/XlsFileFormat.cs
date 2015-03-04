// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Formats
{
    using Accord.IO;
    using System;
    using System.Data;
    using System.IO;

    /// <summary>
    ///   File format for data tables stored as Excel 97-2003 (xls) files.
    /// </summary>
    /// 
    public class XlsFileFormat : FileFormatBase, IFileFormat
    {

        /// <summary>
        /// Gets the file's default extension.
        /// </summary>
        public override string Extension { get { return "*.xls"; } }

        /// <summary>
        /// Gets the format's description.
        /// </summary>
        public override string Description { get { return "Excel worksheets"; } }

        /// <summary>
        /// Gets a value indicating whether this format can be read by this application.
        /// </summary>
        public bool CanRead { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether this format can be written by this application.
        /// </summary>
        public bool CanWrite { get { return false; } }

        /// <summary>
        /// Reads the specified file or stream into a table.
        /// </summary>
        public DataTable Read(Stream stream)
        {
            ExcelReader reader = new ExcelReader(stream, false, true);
            return reader.GetWorksheet(0);
        }

        /// <summary>
        /// Writes the specified table into a file or stream.
        /// </summary>
        public void Write(DataTable table, Stream stream)
        {
            throw new NotSupportedException();
        }
    }

}
