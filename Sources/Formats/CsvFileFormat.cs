// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Formats
{
    using Accord.IO;
    using System.Data;
    using System.IO;

    /// <summary>
    ///   Format handler for data tables stored as comma-separated value (CSV) files.
    /// </summary>
    /// 
    public class CsvFileFormat : FileFormatBase, IFileFormat
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="CsvFileFormat"/> class.
        /// </summary>
        /// 
        public CsvFileFormat()
            : base(extension: "*.csv", description: "Comma-separated values", canRead: true, canWrite: true)
        {
        }

        /// <summary>
        ///   Reads the specified file or stream into a table.
        /// </summary>
        /// 
        public DataTable Read(Stream stream)
        {
            var reader = new CsvReader(new StreamReader(stream), true);
            return reader.ToTable();
        }

        /// <summary>
        ///   Writes the specified table into a file or stream.
        /// </summary>
        /// 
        public void Write(DataTable table, Stream stream)
        {
            var writer = new CsvWriter(new StreamWriter(stream));
            writer.Write(table);
        }
    }
}
