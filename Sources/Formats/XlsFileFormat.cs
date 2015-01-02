using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Accord.IO;
using Accord.IO.Csv;

namespace Statistics_Workbench.Formats
{
    public class XlsFileFormat : FileFormatBase, IFileFormat
    {

        public override string Extension { get { return "*.xls"; } }

        public override string Description { get { return "Excel worksheets"; } }

        public override bool CanRead { get { return true; } }

        public override bool CanWrite { get { return false; } }

        public override DataTable Read(Stream stream)
        {
            ExcelReader reader = new ExcelReader(stream, false, true);
            return reader.GetWorksheet(0);
        }

        public override void Write(DataTable table, Stream stream)
        {
            throw new NotSupportedException();
        }
    }

}
