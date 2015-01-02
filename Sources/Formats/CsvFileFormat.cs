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
