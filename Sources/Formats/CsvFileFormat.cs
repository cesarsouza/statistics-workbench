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

        public override string Extension { get { return "*.csv"; } }

        public override string Description { get { return "Comma-separated values"; } }

        public override bool CanRead { get { return true; } }

        public override bool CanWrite { get { return true; } }

        public override DataTable Read(Stream stream)
        {
            var reader = new CsvReader(new StreamReader(stream), true);

            return reader.ToTable();
        }

        public override void Write(DataTable table, Stream stream)
        {
            var writer = new CsvWriter(new StreamWriter(stream));
            writer.Write(table);
        }
    }

}
