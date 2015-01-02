using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.IO.Csv;

namespace Statistics_Workbench.Formats
{
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
