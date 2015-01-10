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

        public override string Extension { get { return "*.tsv"; } }

        public override string Description { get { return "Tab-separated values"; } }

        public override bool CanRead { get { return true; } }

        public override bool CanWrite { get { return true; } }

        public override DataTable Read(Stream stream)
        {
            var reader = new CsvReader(new StreamReader(stream), true);

            return reader.ToTable();
        }

        public override void Write(DataTable table, Stream stream)
        {
            var writer = new CsvWriter(new StreamWriter(stream), '\t');
            writer.Write(table);
        }
    }

}
