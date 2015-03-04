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
    ///   File format for data tables stored as Excel 2007 (xlsx) files.
    /// </summary>
    /// 
    public class XlsxFileFormat : FileFormatBase, IFileFormat
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="XlsxFileFormat"/> class.
        /// </summary>
        /// 
        public XlsxFileFormat()
            : base(extension: "*.xlsx", description: "Excel 2007+", canRead: true, canWrite: false)
        {
        }

        /// <summary>
        ///   Reads the specified file or stream into a table.
        /// </summary>
        /// 
        public DataTable Read(Stream stream)
        {
            ExcelReader reader = new ExcelReader(stream, true, true);
            return reader.GetWorksheet(0);
        }

        /// <summary>
        ///   Writes the specified table into a file or stream.
        /// </summary>
        /// 
        public void Write(DataTable table, Stream stream)
        {
            throw new NotSupportedException();
        }
    }
}
