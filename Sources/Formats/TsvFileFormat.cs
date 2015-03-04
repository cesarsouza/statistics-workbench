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
    ///   File format for data tables stored as tab-separated value (TSV) files.
    /// </summary>
    /// 
    public class TsvFileFormat : FileFormatBase, IFileFormat
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="TsvFileFormat"/> class.
        /// </summary>
        /// 
        public TsvFileFormat()
            : base(extension: "*.tsv", description: "Tab-separated values", canRead: true, canWrite: true)
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
            var writer = new CsvWriter(new StreamWriter(stream), '\t');
            writer.Write(table);
        }
    }

}
